using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pulumi;

namespace PulumiInfra.Config;

public record GlobalConfig(
    ServiceConfig ServiceConfig,
    ApiConfig ApiConfig,
    StaticSiteConfig StaticSiteConfig,
    PersistenceConfig PersistenceConfig
    )
{
    public static async Task<GlobalConfig> LoadAsync(Pulumi.Config config)
    {
        var azureClientConfig = await Pulumi.AzureNative.Authorization.GetClientConfig.InvokeAsync();

        var apiConfig = new ApiConfigDto
        {
            ClientConfig = azureClientConfig,
            Location = config.Require("location"),
            ResourceGroupName = config.Require("azure-resource-group-name"),
            FunctionsPackagePath = config.Require("functions-package-path")
        };

        var staticSiteConfig = new StaticSiteConfigDto
        {
            StaticSitePath = config.Require("static-site-path")
        };

        var persistenceConfig = new PersistenceConfigDto
        {
            SqlAdminPassword = config.Require("sql-admin-password")
        };

        return new GlobalConfig(
            ServiceConfig: config.RequireObject<ServiceConfigDto>("service-config").GenerateValidConfigObject(),
            ApiConfig: apiConfig.GenerateValidConfigObject(),
            StaticSiteConfig: staticSiteConfig.GenerateValidConfigObject(),
            PersistenceConfig: persistenceConfig.GenerateValidConfigObject()
            );
    }
}

