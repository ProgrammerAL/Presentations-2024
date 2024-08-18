var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql");

var sqlDb = sqlServer.AddDatabase("sqldb");

var internalApiService = builder.AddProject<Projects.Demo_InternalApiService>("internal-api")
                                .WithReference(sqlDb);

var publicApiService = builder.AddProject<Projects.Demo_PublicApiService>("public-api")
                              .WithReference(internalApiService);

builder.Build().Run();
