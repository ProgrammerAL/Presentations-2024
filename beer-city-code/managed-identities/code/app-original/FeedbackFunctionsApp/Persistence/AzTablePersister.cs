using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Data.Tables;
using Azure.Identity;

using Microsoft.Extensions.Options;

namespace FeedbackFunctionsApp.Persistence;

public interface IAzTablePersister
{
    ValueTask StoreItemAsync(string comments);
}

public class AzTablePersister : IAzTablePersister
{
    private readonly IOptions<StorageConfig> _storageConfig;

    public AzTablePersister(IOptions<StorageConfig> storageConfig)
    {
        _storageConfig = storageConfig;
    }

    public async ValueTask StoreItemAsync(string comments)
    {
        var itemKey = Guid.NewGuid().ToString();

        var connectionString = _storageConfig.Value.TableEndpoint;
        var tableClient = new TableClient(connectionString, tableName: "Comments");

        await tableClient.CreateIfNotExistsAsync();

        // Make a dictionary entity by defining a <see cref="TableEntity">.
        var tableEntity = new TableEntity(itemKey, itemKey)
        {
            { "Comments", comments },
        };

        await tableClient.AddEntityAsync(tableEntity);
    }
}
