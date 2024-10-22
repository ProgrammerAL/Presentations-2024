using Cake.Core;

using static AppPaths;

public record AppPaths(
    string SlnFile,
    string UnitTestsCsProj,
    DotNetProject AzFunctionsProject,
    DotNetProject WebClientProject)
{
    public static AppPaths LoadFromContext(ICakeContext context, string buildConfiguration, string srcDirectory, string buildArtifactsPath)
    {
        var slnFile = $"{srcDirectory}/FeedbackApp.sln";
        var unitTestsCsProj = $"{srcDirectory}/UnitTests/UnitTests.csproj";

        var azFunctionsProject = new DotNetProject(
            CsprojFile: $"{srcDirectory}/FeedbackFunctionsApp/FeedbackFunctionsApp.csproj",
            OutDir: $"{srcDirectory}/FeedbackFunctionsApp/bin/{buildConfiguration}/cake-build-output",
            ZipOutDir: buildArtifactsPath,
            ZipOutFilePath:  $"{buildArtifactsPath}/feedback-functions.zip");

        var webClientProject = new DotNetProject(
            CsprojFile: $"{srcDirectory}/FeedbackWebApp/FeedbackWebApp.csproj",
            OutDir: $"{srcDirectory}/FeedbackWebApp/bin/{buildConfiguration}/cake-build-output",
            ZipOutDir: buildArtifactsPath,
            ZipOutFilePath: $"{buildArtifactsPath}/feedback-web-client.zip");

        return new AppPaths(
            slnFile,
            unitTestsCsProj,
            azFunctionsProject,
            webClientProject);
    }

    public record DotNetProject(string CsprojFile, string OutDir, string ZipOutDir, string ZipOutFilePath);
}
