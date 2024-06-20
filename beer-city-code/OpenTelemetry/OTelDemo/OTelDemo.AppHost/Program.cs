var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql");

var sqlDb = sqlServer.AddDatabase("sqldb");

var internalApiService = builder.AddProject<Projects.OTelDemo_InternalApiService>("internal-api")
                                .WithReference(sqlDb);

var publicApiService = builder.AddProject<Projects.OTelDemo_PublicApiService>("public-api")
                              .WithReference(internalApiService);

builder.AddProject<Projects.OTelDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(publicApiService);

builder.Build().Run();
