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
    PersistentStorageResources.PersistentStorageInfra StorageInfra)
{
    public record PersistentStorageInfra(StorageAccount StorageAccount, Output<string> StorageConnectionString);
}

public record PersistentStorageBuilder(
    GlobalConfig GlobalConfig,
    ResourceGroup ResourceGroup)
{
    public PersistentStorageResources Build()
    {
        var storageInfra = GenerateStorageInfrastructure();
        return new PersistentStorageResources(storageInfra);
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
}
