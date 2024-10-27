using System;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Common;
using Cake.Common.IO;
using System.IO;
using System.IO.Compression;
using Cake.Common.Tools.DotNet;
using System.Threading;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Publish;
using System.Collections.Generic;
using Cake.Common.Tools.DotNet.Test;
using System.Net;

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
    public string Target { get; }
    public string BuildConfiguration { get; }
    public string SrcDirectoryPath { get; }
    public string BuildArtifactsPath { get; }
    public AppPaths AppPaths { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Target = context.Argument("target", "Default");
        BuildConfiguration = context.Argument<string>("buildConfiguration");
        SrcDirectoryPath = context.Argument<string>("srcDirectoryPath");
        BuildArtifactsPath = context.Argument<string>("buildArtifactsPath");

        AppPaths = AppPaths.LoadFromContext(context, BuildConfiguration, SrcDirectoryPath, BuildArtifactsPath);
    }
}

[TaskName(nameof(OutputParametersTask))]
public sealed class OutputParametersTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Log.Information($"INFO: Current Working Directory: {context.Environment.WorkingDirectory}");

        context.Log.Information($"INFO: {nameof(context.BuildConfiguration)}: {context.BuildConfiguration}");
        context.Log.Information($"INFO: {nameof(context.SrcDirectoryPath)}: {context.SrcDirectoryPath}");
    }
}

[IsDependentOn(typeof(OutputParametersTask))]
[TaskName(nameof(CleanTask))]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectory(context.AppPaths.AzFunctionsProject.OutDir);
        context.CleanDirectory(context.AppPaths.WebClientProject.OutDir);
    }
}

[IsDependentOn(typeof(CleanTask))]
[TaskName(nameof(BuildTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var buildFuncs = new[]
        {
            () => BuildDotnetApp(context, context.AppPaths.SlnFile),
        };

        var runner = Parallel.ForEach(buildFuncs, func => func());
        while (!runner.IsCompleted)
        {
            Thread.Sleep(100);
        }
    }

    private void BuildDotnetApp(BuildContext context, string pathToProj)
    {
        context.DotNetRestore(pathToProj);

        context.DotNetBuild(pathToProj, new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = context.BuildConfiguration
        });
    }
}

[IsDependentOn(typeof(BuildTask))]
[TaskName(nameof(RunUnitTestsTask))]
public sealed class RunUnitTestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var testSettings = new DotNetTestSettings()
        {
            Configuration = context.BuildConfiguration,
            NoBuild = true,
            ArgumentCustomization = (args) => args.Append("/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura --logger trx")
        };

        var runTestsFuncs = new[]
        {
            () => context.DotNetTest(context.AppPaths.UnitTestsCsProj, testSettings),
        };

        var runner = Parallel.ForEach(runTestsFuncs, func => func());
        while (!runner.IsCompleted)
        {
            Thread.Sleep(100);
        }
    }
}

[IsDependentOn(typeof(RunUnitTestsTask))]
[TaskName(nameof(PublishApplicationsTask))]
public sealed class PublishApplicationsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var buildFuncs = new List<Action>
        {
            () => PublishWebClient(context, context.AppPaths.WebClientProject),
            () => PublishAzureFunctionsProject(context, context.AppPaths.AzFunctionsProject),
        };

        var runner = Parallel.ForEach(buildFuncs, func => func());
        while (!runner.IsCompleted)
        {
            Thread.Sleep(100);
        }
    }

    private void PublishWebClient(BuildContext context, AppPaths.DotNetProject webClient)
    {
        var settings = new DotNetPublishSettings()
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = context.BuildConfiguration,
            OutputDirectory = webClient.OutDir,
        };

        context.DotNetPublish(webClient.CsprojFile, settings);

        //Now that the code is published, create the compressed folder
        if (!Directory.Exists(webClient.ZipOutDir))
        {
            _ = Directory.CreateDirectory(webClient.ZipOutDir);
        }

        if (File.Exists(webClient.ZipOutFilePath))
        {
            File.Delete(webClient.ZipOutFilePath);
        }

        ZipFile.CreateFromDirectory(webClient.OutDir, webClient.ZipOutFilePath);
        context.Log.Information($"Output web app zip file to: {webClient.ZipOutFilePath}");
    }

    private void PublishAzureFunctionsProject(BuildContext context, AppPaths.DotNetProject funcsProj)
    {
        var settings = new DotNetPublishSettings()
        {
            NoRestore = false,
            NoBuild = false,
            Configuration = context.BuildConfiguration,
            OutputDirectory = funcsProj.OutDir,
            Runtime = "linux-x64",
        };

        context.DotNetPublish(funcsProj.CsprojFile, settings);

        //Now that the code is published, create the compressed folder
        if (!Directory.Exists(funcsProj.ZipOutDir))
        {
            _ = Directory.CreateDirectory(funcsProj.ZipOutDir);
        }

        if (File.Exists(funcsProj.ZipOutFilePath))
        {
            File.Delete(funcsProj.ZipOutFilePath);
        }

        ZipFile.CreateFromDirectory(funcsProj.OutDir, funcsProj.ZipOutFilePath);
        context.Log.Information($"Output functions zip file to: {funcsProj.ZipOutFilePath}");
    }
}

[IsDependentOn(typeof(PublishApplicationsTask))]
[TaskName("Default")]
public class DefaultTask : FrostingTask
{
}
