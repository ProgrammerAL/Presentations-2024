using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using Azure.Identity;

namespace FeedbackFunctionsApp.Persistence;

public interface ISqlPersister
{
    ValueTask StoreItemAsync(string comments);
}

public class SqlPersister : ISqlPersister
{
    private readonly IOptions<StorageConfig> _storageConfig;

    public SqlPersister(IOptions<StorageConfig> storageConfig)
    {
        _storageConfig = storageConfig;
    }

    public async ValueTask StoreItemAsync(string comments)
    {
        using var connection = new SqlConnection(_storageConfig.Value.SqlConnectionString);
        await connection.ExecuteAsync("INSERT INTO [dbo].[COMMENTS] (Comments) VALUES (@comments)", new { comments });
    }
}
