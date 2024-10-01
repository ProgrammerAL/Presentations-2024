---
marp: true
title: Doing DevOps like a Dev
paginate: true
theme: gaia
author: Al Rodriguez
---

# Doing DevOps like a Dev

with Senior Azure Cloud Engineer at Microsoft
AL Rodriguez
aka @ProgrammerAL
aka ProgrammerAL.com

---

# Dev == Automation

- Automate more
  - More than that
    - And more than that

---

# YAML is a DSL

- Domain Specific Language (DSL)
- It's different everywhere
  - Custom to each provider

---

# YAML "Runs" in the cloud

- Long feedback loops
- Can't test locally

---

# General Purpose Programming Languages to the Rescue

- Write more code!
- Specialty Frameworks
  - C# Cake, C# Nuke, PowerShell, Dagger.io
- Or whatever you want

---

# Example: GitHub Actions with C# Cake

```yaml
steps:
  - uses: actions/checkout@v2
  - name: Setup .NET
    uses: actions/setup-dotnet@v1
    with:
      dotnet-version: ${{ env.DOTNET_VERSION }}

  - name: Cake - Build
    run: dotnet run --project build/build/Build.csproj

  - name: Cake - Deploy
    run: dotnet run --project ${{ github.workspace }}/deploy/deploy/Deploy.csproj
```

---

# Automate Manual Processes for Deployment

- Find manual processes and automate them
  - Easy right?

---

# Azure Cosmos DB

- Document Database
- HTTP Requests

---

# Cosmos DB Charges per Operation

- Every request has a Request Unit charge
- More work == More Expensive

---

# Cosmos DB Indexes Everything

- All properties indexed by default
- Bigger document == More Work == More Expensive

---

# Sample Document

```json
{
  "locations": [
    { "country": "Germany", "city": "Berlin" },
    { "country": "France", "city": "Paris" }
  ],
  "headquarters": { "country": "Belgium", "employees": 250 },
  "exports": [
    { "city": "Moscow" },
    { "city": "Athens" }
  ]
}
```
---

# Devs Manage Indexes Manually

- Devs add new property that should be indexed
  - Manually updated in each environment
  - Manually tracked in tickets

---

# Fix with Custom Solution

- Devs:
  - Add custom code to solution
  - Specify what properties should be indexed
    - Default to not indexed
- DevOps:
  - Scan compiled code for indexes during build
    - Generate metadata.json file
  - Upload index properties to Cosmos DB during deploy

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

    public string id => EntityId;

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

var dbMappedIndexes = new CosmosDbIndexMapper().MapIndexes(assembly);

var cosmosDbMetadata = new ApiCosmosDbMetadata(dbMappedIndexes);

File.WriteAllText($"{context.ArcadeMachineManagementApiPaths.BuildMetadataOutputPath}/cosmos-db-indexes-metadata.json", JsonSerializer.Serialize(cosmosDbMetadata));
```

---

# Deploy Pushes CosmosDB Indexes

```csharp
var filePath = File.ReadAllText(deploymentPackagesConfig.ArcadeMachineManagementServiceCosmosMetadata);
var metadata = System.Text.Json.JsonSerializer.Deserialize<ApiCosmosDbMetadata>(filePath);

PushIndexesToCosmosDb(metadata.Indexes);
```

---

# Key Takeaways

- More automation
- Mode code
- Less config/DSLs

---

# Me (AL)

- @ProgrammerAL
- ProgrammerAL.com
- Senior Azure Cloud Engineer at Microsoft

---
