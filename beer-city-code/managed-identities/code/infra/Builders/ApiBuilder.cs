using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

using PulumiInfra.Config;

using static PulumiInfra.Builders.ApiResources;

using AzureNative = Pulumi.AzureNative;

namespace PulumiInfra.Builders;

public record ApiResources(ApiResources.ServiceStorageInfra ServiceStorage, ApiResources.FunctionInfra Function)
{
    public record ServiceStorageInfra(StorageAccount StorageAccount, BlobContainer FuncsContainer, Blob FuncsBlob);
    public record FunctionInfra(WebApp WebApp, Output<string> HttpsEndpoint);
}

public record ApiBuilder(
    GlobalConfig GlobalConfig,
    ResourceGroup ResourceGroup,
    PersistentStorageResources PersistenceResources)
{
    public ApiResources Build()
    {
        var storageInfra = GenerateStorageInfrastructure();
        var functionsInfra = GenerateFunctionsInfrastructure(storageInfra);

        return new ApiResources(storageInfra, functionsInfra);
    }

    private ApiResources.ServiceStorageInfra GenerateStorageInfrastructure()
    {
        var storageAccount = new StorageAccount("funcsstorage", new StorageAccountArgs
        {
            ResourceGroupName = ResourceGroup.Name,
            Sku = new AzureNative.Storage.Inputs.SkuArgs
            {
                Name = AzureNative.Storage.SkuName.Standard_LRS,
            },
            Kind = Kind.StorageV2,
            EnableHttpsTrafficOnly = true,
            MinimumTlsVersion = MinimumTlsVersion.TLS1_2,
            AccessTier = AccessTier.Hot,
            AllowSharedKeyAccess = true,
            SasPolicy = new AzureNative.Storage.Inputs.SasPolicyArgs
            {
                ExpirationAction = ExpirationAction.Log,
                SasExpirationPeriod = "00.01:00:00"
            }
        });

        var container = new BlobContainer("funcscontainer", new BlobContainerArgs
        {
            ResourceGroupName = ResourceGroup.Name,
            AccountName = storageAccount.Name,
            PublicAccess = PublicAccess.None,
        });

        var blob = new Blob("funcs-blob", new BlobArgs
        {
            BlobName = "functions.zip",
            ResourceGroupName = ResourceGroup.Name,
            AccountName = storageAccount.Name,
            ContainerName = container.Name,
            Type = BlobType.Block,
        });

        return new ApiResources.ServiceStorageInfra(storageAccount, container, blob);
    }

    private ApiResources.FunctionInfra GenerateFunctionsInfrastructure(ServiceStorageInfra storageInfra)
    {
        //Create the App Service Plan
        var appServicePlan = new AppServicePlan("functions-app-service-plan", new AppServicePlanArgs
        {
            ResourceGroupName = ResourceGroup.Name,
            Kind = "Linux",
            Sku = new SkuDescriptionArgs
            {
                Tier = "Dynamic",
                Name = "Y1"
            },
            // For Linux, you need to change the plan to have Reserved = true property.
            Reserved = true,
        });

        var storageAccountConnectionString = GetStorageAccountConnectionString(storageInfra);
        
        var functionAppSiteConfig = new SiteConfigArgs
        {
            LinuxFxVersion = "DOTNET-ISOLATED|8.0",
            Cors = new CorsSettingsArgs
            {
                AllowedOrigins = new[] { "*" }
            },
            AppSettings = new[]
            {
                new NameValuePairArgs
                {
                    Name = "FUNCTIONS_WORKER_RUNTIME",
                    Value = "dotnet-isolated",
                },
                new NameValuePairArgs
                {
                    Name = "FUNCTIONS_EXTENSION_VERSION",
                    Value = "~4",
                },
                new NameValuePairArgs
                {
                    Name = "AzureWebJobsStorage",
                    Value = storageAccountConnectionString,
                },
                new NameValuePairArgs
                {
                    Name = "WEBSITE_RUN_FROM_PACKAGE",
                    Value = storageInfra.FuncsBlob.Url,
                },
                new NameValuePairArgs
                {
                    Name = "SCM_DO_BUILD_DURING_DEPLOYMENT",
                    Value = "0"
                },
                new NameValuePairArgs
                {
                    Name = "ServiceConfig__Version",
                    Value = GlobalConfig.ServiceConfig.Version
                },
                new NameValuePairArgs
                {
                    Name = "ServiceConfig__Environment",
                    Value = GlobalConfig.ServiceConfig.Environment,
                },
                new NameValuePairArgs
                {
                    Name = "StorageConfig__TableEndpoint",
                    Value = PersistenceResources.StorageInfra.StorageConnectionString,
                },
                new NameValuePairArgs
                {
                    Name = "StorageConfig__TableName",
                    Value = "Comments",
                }
            }
        };

        //Create the App Service
        var webApp = new WebApp("functions-app", new WebAppArgs
        {
            Kind = "FunctionApp",
            ResourceGroupName = ResourceGroup.Name,
            ServerFarmId = appServicePlan.Id,
            HttpsOnly = true,
            SiteConfig = functionAppSiteConfig,
            ClientAffinityEnabled = false,
        });

        var httpsEndpoint = webApp.DefaultHostName.Apply(x => $"https://{x}");

        return new ApiResources.FunctionInfra(
            webApp,
            httpsEndpoint);
    }

    private Output<string> GetStorageAccountConnectionString(ServiceStorageInfra serviceStorageInfra)
    {
        var storageKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
        {
            ResourceGroupName = ResourceGroup.Name,
            AccountName = serviceStorageInfra.StorageAccount.Name,
        });

        var storageKey = storageKeys.Apply(x => x.Keys.First().Value);

        return Output.Tuple(
            serviceStorageInfra.StorageAccount.Name,
            storageKey)
            .Apply(items =>
            {
                var accountName = items.Item1;
                var key = items.Item2;

                return $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={key};EndpointSuffix=core.windows.net";
            });
    }
}
