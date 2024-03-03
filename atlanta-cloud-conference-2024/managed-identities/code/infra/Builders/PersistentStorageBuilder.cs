using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

using Pulumi;
using Pulumi.AzureNative;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;

using PulumiInfra.Config;

using AzureNative = Pulumi.AzureNative;

namespace PulumiInfra.Builders;

public record PersistentStorageResources(
    PersistentStorageResources.PersistentStorageInfra StorageInfra,
    PersistentStorageResources.SqlStorageInfra SqlInfra)
{
    public record PersistentStorageInfra(StorageAccount StorageAccount, Output<string> StorageConnectionString);
    public record SqlStorageInfra(
        AzureNative.Sql.Server SqlServer,
        AzureNative.Sql.Database SqlDatabase,
        AzureNative.Sql.FirewallRule AlowAllFirewallRule,
        Output<string> SqlConnectionString
        );
}

public record PersistentStorageBuilder(
    GlobalConfig GlobalConfig,
    ResourceGroup ResourceGroup)
{
    public PersistentStorageResources Build()
    {
        var storageInfra = GenerateStorageInfrastructure();
        var sqlInfra = GenerateSqlInfrastructure();
        return new PersistentStorageResources(storageInfra, sqlInfra);
    }

    private PersistentStorageResources.PersistentStorageInfra GenerateStorageInfrastructure()
    {
        var storageAccount = new StorageAccount("persistentstg", new StorageAccountArgs
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
            AllowSharedKeyAccess = true
        });

        var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
        {
            ResourceGroupName = ResourceGroup.Name,
            AccountName = storageAccount.Name
        });

        var primaryStorageKey = storageAccountKeys.Apply(accountKeys =>
        {
            var firstKey = accountKeys.Keys[0].Value;
            return firstKey;
        });

        var storageConnectionString =
            Output.All(storageAccount.Name, primaryStorageKey).Apply(x =>
            {
                var storageAccountName = x[0];
                var primaryStorageKey = x[1];

                return $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={primaryStorageKey};EndpointSuffix=core.windows.net";
            });

        return new PersistentStorageResources.PersistentStorageInfra(storageAccount, storageConnectionString);
    }

    private PersistentStorageResources.SqlStorageInfra GenerateSqlInfrastructure()
    {
        var sqlServer = new AzureNative.Sql.Server("sql-server", new AzureNative.Sql.ServerArgs
        {
            ResourceGroupName = ResourceGroup.Name,
            Location = ResourceGroup.Location,
            //ServerName = "MySqlServer",
            AdministratorLogin = "MySqlAdminUser",
            AdministratorLoginPassword = GlobalConfig.PersistenceConfig.SqlAdminPassword,
            //Version = "12.0", // Specify the server version, here we're using SQL Server 2019
            PublicNetworkAccess = AzureNative.Sql.ServerNetworkAccessFlag.Enabled
        });

        var sqlDatabase = new AzureNative.Sql.Database("sql-database", new AzureNative.Sql.DatabaseArgs
        {
            DatabaseName = "mydatabase",
            ResourceGroupName = ResourceGroup.Name,
            ServerName = sqlServer.Name,
            Location = ResourceGroup.Location,
            Sku = new AzureNative.Sql.Inputs.SkuArgs
            {
                Name = "Basic",
                Tier = "Basic"
            }
        });

        // Firewall rule to allow an IP to connect directly to the SQL Server
        //  Bad practice, and only used here to make the demo easy
        //  Don't do this in real life I guess
        var sqlFirewallRule = new AzureNative.Sql.FirewallRule("allow-all-sql-firewall-rule", new AzureNative.Sql.FirewallRuleArgs
        {
            ResourceGroupName = ResourceGroup.Name,
            ServerName = sqlServer.Name,
            StartIpAddress = "0.0.0.0",
            EndIpAddress = "255.255.255.255",
            FirewallRuleName = "AllowAllWindowsAzureIps",
        });

        var sqlConnectionString = Output.All(
            sqlServer.FullyQualifiedDomainName,
            sqlServer.AdministratorLogin!,
            sqlDatabase.Name
            ).Apply(x =>
            {
                var serverDomainName = x[0];
                var adminUser = x[1];
                var adminPassword = GlobalConfig.PersistenceConfig.SqlAdminPassword;
                var dbName = x[2];

                return $"Server=tcp:{serverDomainName},1433;Initial Catalog={dbName};Persist Security Info=False;User ID={adminUser};Password={adminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            });

        return new PersistentStorageResources.SqlStorageInfra(sqlServer, sqlDatabase, sqlFirewallRule, sqlConnectionString);
    }
}
