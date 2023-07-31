
using Common.Orleans;
using Microsoft.Azure.Cosmos;
using Orleans.Clustering.Cosmos;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddWebAppApplicationInsights("Dashboard");
builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    var cosmosDbKey = ctx.Configuration.GetValue<string>("CosmosDbKey");
    var cosmosDbUri = ctx.Configuration.GetValue<string>("CosmosDbUri");
    
    siloBuilder
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = "Cluster";
            options.ServiceId = "Service";
        })
        .Configure<SiloOptions>(options =>
        {
            options.SiloName = "Dashboard";
        })
        .ConfigureEndpoints(siloPort: 11_112, gatewayPort: 30_001)
        .UseDashboard(config => 
            config.HideTrace = 
                !string.IsNullOrEmpty(builder.Configuration.GetValue<string>("HideTrace")) 
                    ? builder.Configuration.GetValue<bool>("HideTrace") 
                    : true);

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

// uncomment this if you dont mind hosting grains in the dashboard
builder.Services.DontHostGrainsHere();

var app = builder.Build();

app.MapGet("/", () => Results.Ok("Dashboard"));

app.Run();
