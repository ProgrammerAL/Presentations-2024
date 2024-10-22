using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

using Pulumi.Automation;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string WorkspacePath { get; }
    public string PulumiPath { get; }
    public string PulumiStackName { get; }
    public string PlaywrightTestsPath { get; }
    public string PlaywrightBrowser { get; }
    public string ReleaseVersion { get; }
    public string ReleaseArtifactsDownloadDir { get; }
    public string UnzippedArtifactsDir { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        WorkspacePath = LoadParameter(context, nameof(WorkspacePath));
        PulumiStackName = LoadParameter(context, nameof(PulumiStackName));
        ReleaseVersion = LoadParameter(context, nameof(ReleaseVersion));
        PulumiPath = LoadParameter(context, nameof(PulumiPath));
        PlaywrightTestsPath = LoadParameter(context, nameof(PlaywrightTestsPath));
        PlaywrightBrowser = LoadParameter(context, nameof(PlaywrightBrowser));

        ReleaseArtifactsDownloadDir = LoadParameter(context, nameof(ReleaseArtifactsDownloadDir));
        UnzippedArtifactsDir = WorkspacePath + "/unzipped_artifacts";
    }

    private string LoadParameter(ICakeContext context, string parameterName)
    {
        return context.Arguments.GetArgument(parameterName) ?? throw new Exception($"Missing parameter '{parameterName}'");
    }
}

[TaskName(nameof(UnzipAssetsTask))]
public sealed class UnzipAssetsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        _ = Directory.CreateDirectory(context.UnzippedArtifactsDir);

        ExtractArchive("feedback-web-client", context);
    }

    private void ExtractArchive(string zipName, BuildContext context)
    {
        var zipFilePath = $"{context.ReleaseArtifactsDownloadDir}/{zipName}.zip";
        var outputPath = $"{context.UnzippedArtifactsDir}/{zipName}";

        context.Log.Information($"Extracting zip '{zipFilePath}' to '{outputPath}'");

        using var fileStream = File.OpenRead(zipFilePath);
        using var archive = new ZipArchive(fileStream);
        archive.ExtractToDirectory(outputPath);
    }

    private void CopyArchive(string zipName, BuildContext context)
    {
        var zipFilePath = $"{context.ReleaseArtifactsDownloadDir}/{zipName}.zip";
        var outputPath = $"{context.UnzippedArtifactsDir}/{zipName}.zip";

        context.Log.Information($"Copying zip '{zipFilePath}' to '{outputPath}'");
        File.Copy(sourceFileName: zipFilePath, destFileName: outputPath);
    }
}

[IsDependentOn(typeof(UnzipAssetsTask))]
[TaskName(nameof(UpdatePulumiConfigTask))]
public sealed class UpdatePulumiConfigTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var configFilePath = $"{context.PulumiPath}/Pulumi.{context.PulumiStackName}.yaml";
        var configFileText = File.ReadAllText(configFilePath);

        configFileText = UpdateConfigValue("update-conf-2024:functions-package-path: ", $"{context.ReleaseArtifactsDownloadDir}/feedback-functions.zip", configFileText);
        configFileText = UpdateConfigValue("update-conf-2024:static-site-path: ", $"{context.UnzippedArtifactsDir}/feedback-web-client", configFileText);
        configFileText = UpdateConfigValue("Version: ", context.ReleaseVersion, configFileText);

        File.WriteAllText(configFilePath, configFileText);
        context.Log.Information("Pulumi Config: \n" + configFileText);
    }

    private string UpdateConfigValue(string keyName, string newValue, string stringToUpdate)
    {
        var keyIndex = stringToUpdate.IndexOf(keyName);
        if (keyIndex < 0)
        {
            throw new Exception($"Could not find index of text '{keyName}' to update the config value");
        }

        var valueEndIndex = stringToUpdate.IndexOf("local", keyIndex);

        if (valueEndIndex < 0)
        {
            throw new Exception($"Could not find index of text 'local' to update the service version config value");
        }

        //Remove the default value 'local' and add the new value to it
        return stringToUpdate.Remove(valueEndIndex, "local".Length)
                             .Insert(valueEndIndex, $"\"{newValue}\"");
    }
}

[IsDependentOn(typeof(UpdatePulumiConfigTask))]
[TaskName(nameof(PulumiDeployTask))]
public sealed class PulumiDeployTask : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        var fullStackName = $"ProgrammerAL/{context.PulumiStackName}";

        context.Log.Information($"Loading stack {fullStackName} from path '{context.PulumiPath}'");

        var stackArgs = new LocalProgramArgs(fullStackName, context.PulumiPath);
        var pulumiStack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);

        context.Log.Information($"Pulumi Up starting...");

        var result = await pulumiStack.UpAsync();
        context.Log.Information($"Pulumi Up completed");
        Utilities.LogPulumiResult(context, result);
    }
}

[IsDependentOn(typeof(PulumiDeployTask))]
[TaskName(nameof(RunSeleniumTestsTask))]
public sealed class RunSeleniumTestsTask : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        await WriteConfigAsync(context);
        await RunTestsAsync(context);
    }

    private async Task RunTestsAsync(BuildContext context)
    {
        var testSettings = new DotNetTestSettings()
        {
            Configuration = "Debug",
            NoBuild = false,
            NoRestore = false,
            ArgumentCustomization = (args) => args.Append("/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura --logger trx"),
            EnvironmentVariables = new Dictionary<string, string>
            {
                { "BROWSER", context.PlaywrightBrowser }
            }
        };

        var projectPath = $"{context.PlaywrightTestsPath}/UiTests.csproj";
        context.DotNetTest(projectPath, testSettings);

        await Task.CompletedTask;
    }

    private async ValueTask WriteConfigAsync(BuildContext context)
    {
        var fullStackName = $"ProgrammerAL/{context.PulumiStackName}";

        context.Log.Information($"Loading stack {fullStackName} from path '{context.PulumiPath}' to get outputs");

        var stackArgs = new LocalProgramArgs(fullStackName, context.PulumiPath);
        var pulumiStack = await LocalWorkspace.SelectStackAsync(stackArgs);
        var stackOutputs = await pulumiStack.GetOutputsAsync();
        var staticSiteEndpoint = stackOutputs["StaticSiteHttpsEndpoint"].Value.ToString() ?? throw new Exception($"Pulumi output 'StaticSiteHttpsEndpoint' is null for stack '{fullStackName}'");


        // Write the config file
        var filePath = $"{context.PlaywrightTestsPath}/.runsettings";
        var runsettingsXml = XElement.Load(filePath);
        var baseUrlElement = runsettingsXml
            .Element("TestRunParameters")
            !.Elements("Parameter")
            .Single(x => string.Equals(x.Attribute("name")?.Value, "baseUrl"));
        baseUrlElement.Value = staticSiteEndpoint;

        File.WriteAllText(filePath, runsettingsXml.ToString());
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(RunSeleniumTestsTask))]
public class DefaultTask : FrostingTask
{
}
