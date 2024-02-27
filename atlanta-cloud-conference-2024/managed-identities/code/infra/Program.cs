using Pulumi;
using Pulumi.AzureNative.Resources;

using PulumiInfra;
using PulumiInfra.Builders;
using PulumiInfra.Config;

using System.Collections.Generic;

return await Pulumi.Deployment.RunAsync(async () =>
{
    var pulumiConfig = new Config();
    var globalConfig = await GlobalConfig.LoadAsync(pulumiConfig);

    var resourceGroup = new ResourceGroup(globalConfig.ApiConfig.ResourceGroupName, new ResourceGroupArgs
    {
        Location = globalConfig.ApiConfig.Location
    });

    var persistentStorageBuilder = new PersistentStorageBuilder(globalConfig, resourceGroup);
    var persistenceResources = persistentStorageBuilder.Build();

    var apiBuilder = new ApiBuilder(globalConfig, resourceGroup, persistenceResources);
    var apiResources = apiBuilder.Build();

    var staticSiteBuilder = new StaticSiteBuilder(globalConfig, resourceGroup, apiResources);
    var staticSiteResources = staticSiteBuilder.Build();

    var sqlConnectionString = Output.All(
        persistenceResources.SqlInfra.SqlServer.FullyQualifiedDomainName,
        persistenceResources.SqlInfra.SqlServer.AdministratorLogin!,
        persistenceResources.SqlInfra.SqlDatabase.Name
        ).Apply(x =>
    {
        var serverDomainName = x[0];
        var adminUser = x[1];
        var adminPassword = globalConfig.PersistenceConfig.SqlAdminPassword;
        var dbName = x[2];

        return $"Server=tcp:{serverDomainName},1433;Initial Catalog={dbName};Persist Security Info=False;User ID={adminUser};Password={adminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    });

    return new Dictionary<string, object?>
    {
        { "Readme", Output.Create(System.IO.File.ReadAllText("./Pulumi.README.md")) },
        { "FunctionHttpsEndpoint", apiResources.Function.HttpsEndpoint },
        { "StaticSiteHttpsEndpoint", staticSiteResources.StorageInfra.SiteStorageAccount.PrimaryEndpoints.Apply(x => x.Web) },
        { "SqlConnectionString", sqlConnectionString },
        { "StorageConnectionString", persistenceResources.StorageInfra.StorageConnectionString }
    };
});

