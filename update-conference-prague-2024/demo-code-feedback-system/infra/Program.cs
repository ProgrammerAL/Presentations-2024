using Pulumi;

using PulumiInfra.Builders;
using PulumiInfra.Config;

using System.Collections.Generic;

return await Pulumi.Deployment.RunAsync(async () =>
{
    var pulumiConfig = new Config();
    var globalConfig = await GlobalConfig.LoadAsync(pulumiConfig);

    var resourceGroupBuilder = new AzureResourceGroupStackBuilder(globalConfig);
    var resourceGroupResources = resourceGroupBuilder.GenerateResources();

    var persistentStorageBuilder = new PersistentStorageBuilder(globalConfig, resourceGroupResources);
    var persistenceResources = persistentStorageBuilder.Build();

    var apiBuilder = new ApiBuilder(globalConfig, resourceGroupResources, persistenceResources);
    var apiResources = apiBuilder.Build();

    var staticSiteBuilder = new StaticSiteBuilder(globalConfig, resourceGroupResources, apiResources);
    var staticSiteResources = staticSiteBuilder.Build();

    return new Dictionary<string, object?>
    {
        { "Readme", Output.Create(System.IO.File.ReadAllText("./Pulumi.README.md")) },
        { "FunctionHttpsEndpoint", apiResources.Function.HttpsEndpoint },
        { "StaticSiteHttpsEndpoint", staticSiteResources.StorageInfra.SiteStorageAccount.PrimaryEndpoints.Apply(x => x.Web) },
    };
});

