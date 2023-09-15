using Abstractions;
using Abstractions.OrleansCommon;
using Microsoft.Azure.Cosmos;
using Orleans.Clustering.Cosmos;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// builder.Services.AddWebAppApplicationInsights("Dashboard");
builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    var cosmosDbKey = ctx.Configuration.GetValue<string>("CosmosDbKey");
    var cosmosDbUri = ctx.Configuration.GetValue<string>("CosmosDbUri");

    siloBuilder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = ClusterConfig.ClusterId;
            options.ServiceId = ClusterConfig.ServiceId;
        })
        .Configure<SiloOptions>(options =>
        {
            options.SiloName = "Dashboard";
        })
        .ConfigureEndpoints(siloPort: 11_112, gatewayPort: 30_001);

    if (cosmosDbKey is { } && cosmosDbUri is { })
    {
        siloBuilder.UseCosmosClustering((CosmosClusteringOptions opt) =>
        {
            opt.IsResourceCreationEnabled = true;
            opt.DatabaseName = CosmosDbConfig.CosmosOrleansDbName;
            opt.ClientOptions = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Direct };
            opt.ConfigureCosmosClient(accountEndpoint: cosmosDbUri, authKeyOrResourceToken: cosmosDbKey);
        });
    }
    else
    {
        siloBuilder.UseLocalhostClustering();
    }
    
});


var app = builder.Build();

app.MapControllers();

app.Run();
