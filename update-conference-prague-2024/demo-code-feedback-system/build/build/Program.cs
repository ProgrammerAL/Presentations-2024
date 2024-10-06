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
    public WebsitePaths WebClientPaths { get; }
    public FeedbackFunctionsProjectPaths AzFunctionsPaths { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Target = context.Argument("target", "Default");
        BuildConfiguration = context.Argument<string>("buildConfiguration");
        SrcDirectoryPath = context.Argument<string>("srcDirectoryPath");
        BuildArtifactsPath = context.Argument<string>("buildArtifactsPath");

        WebClientPaths = WebsitePaths.LoadFromContext(context, BuildConfiguration, SrcDirectoryPath, BuildArtifactsPath);
        AzFunctionsPaths = FeedbackFunctionsProjectPaths.LoadFromContext(context, BuildConfiguration, SrcDirectoryPath, BuildArtifactsPath);
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
        context.CleanDirectory(context.WebClientPaths.OutDir);
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
            () => BuildDotnetApp(context, context.WebClientPaths.CsprojFile),
            () => BuildDotnetApp(context, context.AzFunctionsPaths.CsprojFile),
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
        //TODO: Add some tests
        //var testSettings = new DotNetTestSettings()
        //{
        //    Configuration = context.BuildConfiguration,
        //    NoBuild = true,
        //    ArgumentCustomization = (args) => args.Append("/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura --logger trx")
        //};

        //var runTestsFuncs = new[]
        //{
        //    () => context.DotNetTest(context.WebClientPaths.UnitTestProj, testSettings),
        //};

        //var runner = Parallel.ForEach(runTestsFuncs, func => func());
        //while (!runner.IsCompleted)
        //{
        //    Thread.Sleep(100);
        //}
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
            () => PublishWebClient(context),
            () => PublishAzureFunctionsProject(context, context.AzFunctionsPaths),
        };

        var runner = Parallel.ForEach(buildFuncs, func => func());
        while (!runner.IsCompleted)
        {
            Thread.Sleep(100);
        }
    }

    private void PublishWebClient(BuildContext context)
    {
        var settings = new DotNetPublishSettings()
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = context.BuildConfiguration,
            OutputDirectory = context.WebClientPaths.OutDir,
        };

        context.DotNetPublish(context.WebClientPaths.CsprojFile, settings);

        //Now that the code is published, create the compressed folder
        if (!Directory.Exists(context.WebClientPaths.ZipOutDir))
        {
            _ = Directory.CreateDirectory(context.WebClientPaths.ZipOutDir);
        }

        if (File.Exists(context.WebClientPaths.ZipOutFilePath))
        {
            File.Delete(context.WebClientPaths.ZipOutFilePath);
        }

        ZipFile.CreateFromDirectory(context.WebClientPaths.OutDir, context.WebClientPaths.ZipOutFilePath);
        context.Log.Information($"Output web app zip file to: {context.WebClientPaths.ZipOutFilePath}");
    }

    private void PublishAzureFunctionsProject(BuildContext context, FeedbackFunctionsProjectPaths apiPaths)
    {
        var settings = new DotNetPublishSettings()
        {
            NoRestore = false,
            NoBuild = false,
            Configuration = context.BuildConfiguration,
            OutputDirectory = apiPaths.OutDir,
            Runtime = "linux-x64",
        };

        context.DotNetPublish(apiPaths.CsprojFile, settings);

        //Now that the code is published, create the compressed folder
        if (!Directory.Exists(apiPaths.ZipOutDir))
        {
            _ = Directory.CreateDirectory(apiPaths.ZipOutDir);
        }

        if (File.Exists(apiPaths.ZipOutPath))
        {
            File.Delete(apiPaths.ZipOutPath);
        }

        ZipFile.CreateFromDirectory(apiPaths.OutDir, apiPaths.ZipOutPath);
        context.Log.Information($"Output functions zip file to: {apiPaths.ZipOutPath}");
    }
}

[IsDependentOn(typeof(PublishApplicationsTask))]
[TaskName("Default")]
public class DefaultTask : FrostingTask
{
}
