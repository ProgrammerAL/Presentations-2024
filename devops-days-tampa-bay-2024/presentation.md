---
marp: true
title: Doing DevOps like a Dev
paginate: true
theme: gaia
author: Al Rodriguez
---

# Doing DevOps like a Dev

with AL Rodriguez

---

# Me (AL)

- @ProgrammerAL
- ProgrammerAL.com
- Senior Azure Cloud Engineer at Microsoft

---

# Dev == Automation

- Automate more
  - More than that
    - And more than that

---

# YAML is Everywhere

- A Domain Specific Language (DSL)
- Custom to each provider

---

# YAML "Runs" in the cloud

- Long feedback loops
- Can't test it locally

---

# YAML Tooling isn't as Mature as Other Dev Tools

- Basic syntax highlighting
- Different IDE Extension per provider

---

# General Purpose Programming Languages to the Rescue

- Write more code!
- Specialty Frameworks
  - C# Cake, C# Nuke, PowerShell, Dagger.io
- Or whatever you want

---

# Example: GitHub Actions with C# Cake

```yaml
name: Build and Deploy

on:
  push:
    branches: [main]

jobs:
  build-and-publish:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Cake - Build
        run: dotnet run --project build/build/Build.csproj -- --configuration=${{ env.CONFIGURATION }} --srcDirectoryPath=${{ env.SRC_DIRECTORY_PATH }} --BuildArtifactsPath=${{ env.BUILD_ARTIFACTS_PATH }}

      - name: Upload Artifacts
        id: upload-release-asset
        uses: actions/upload-release-asset@v1
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        with:
          upload_url: ${{ steps.create_release.outputs.upload_url }}
          asset_path: "${{ env.SRC_DIRECTORY_PATH }}/build-out/assets.zip"
          asset_name: assets.zip
          asset_content_type: application/zip

      - name: Cake - Deploy
        run: dotnet run --project ${{ github.workspace }}/deploy/deploy/Deploy.csproj -- --configuration=${{ env.CONFIGURATION }} --WorkspacePath=${{ github.workspace }} --BuildArtifactsPath=${{ env.BUILD_ARTIFACTS_PATH }}
```

---

# Automation Task: Automate Manual Processes for Deployment

- Example: Cosmos DB Indexes
  - Work with Devs to know what indexes should be enabled/disabled

---

# C# Attributes Mark Indexes

```csharp
[IdConfiguredEntity(containerName: ContainerName)]
public class UserEntity
{
    public const string ContainerName = "UserEntities";

    [IncludePartitionKey]
    [IncludeIndex]
    public required string EntityId { get; set; }

    public string id => Version.ToString();

    [IncludeIndex]
    public required string Email{ get; set; }

    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
}
```
---

# Cake Build Generates Indexes JSON file

```csharp
var assemblyPath = Directory.GetFiles(binPath, DbContextAssemblyFileName, SearchOption.AllDirectories).Single();
var assembly = Assembly.LoadFrom(assemblyPath);

var mapper = new CosmosDbIndexMapper();
var dbMappedIndexes = mapper.MapIndexes(assembly);

var metadata = new ApiBuildMetadata
(
    Version: context.Version,
    BuildDate: DateTime.UtcNow,
    BuildConfiguration: context.BuildConfiguration
);

var cosmosDbMetadata = new ApiCosmosDbMetadata(dbMappedIndexes);

File.WriteAllText($"{context.ArcadeMachineManagementApiPaths.BuildMetadataOutputPath}/build-metadata.json", JsonSerializer.Serialize(metadata));
File.WriteAllText($"{context.ArcadeMachineManagementApiPaths.BuildMetadataOutputPath}/cosmos-db-indexes-metadata.json", JsonSerializer.Serialize(cosmosDbMetadata));
```

---

# CI/CD Deployment Pushes CosmosDB Indexes

```csharp
var filePath = File.ReadAllText(deploymentPackagesConfig.ArcadeMachineManagementServiceCosmosMetadata);
var metadata = System.Text.Json.JsonSerializer.Deserialize<ApiCosmosDbMetadata>(filePath, new System.Text.Json.JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
});

if (metadata is null)
{ 
    throw new Exception("Could not deserialize cosmos db metadata");
}

DbMappedIndexes = metadata.Indexes;
```

---

# Key Takeaways

- More automation
- Less config/DSLs

---

# Online Info

- @ProgrammerAL
- programmerAL.com

