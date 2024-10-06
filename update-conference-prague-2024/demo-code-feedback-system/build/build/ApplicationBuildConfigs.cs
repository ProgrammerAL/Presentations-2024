using Cake.Core;

public record WebsitePaths(
    string CsprojFile,
    string OutDir,
    string ZipOutDir,
    string ZipOutFilePath)
{
    public static WebsitePaths LoadFromContext(ICakeContext context, string buildConfiguration, string srcDirectory, string buildArtifactsPath)
    {
        var projectName = "FeedbackWebApp";
        var projectDir = srcDirectory + $"/{projectName}";
        var cprojFile = projectDir + $"/{projectName}.csproj";
        var outDir = projectDir + $"/bin/{buildConfiguration}/cake-build-output";
        var zipOutDir = buildArtifactsPath;
        var zipOutFilePath = zipOutDir + $"/feedback-web-client.zip";

        return new WebsitePaths(
            cprojFile,
            outDir,
            zipOutDir,
            zipOutFilePath);
    }
}

public record FeedbackFunctionsProjectPaths(
    string CsprojFile, 
    string OutDir, 
    string ZipOutDir, 
    string ZipOutPath)
{
    public static FeedbackFunctionsProjectPaths LoadFromContext(ICakeContext context, string buildConfiguration, string srcDirectory, string buildArtifactsPath)
    {
        var projectName = "FeedbackFunctionsApp";
        var projectDir = srcDirectory + $"/{projectName}";
        var csprojFile = projectDir + $"/{projectName}.csproj";
        var outDir = projectDir + $"/bin/{buildConfiguration}/cake-build-output";
        var zipOutDir = buildArtifactsPath;
        var zipOutFilePath = zipOutDir + $"/feedback-functions.zip";

        return new FeedbackFunctionsProjectPaths(
            csprojFile,
            outDir,
            zipOutDir,
            zipOutFilePath);
    }
}