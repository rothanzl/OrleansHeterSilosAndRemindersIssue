using Abstractions;
using Common.Orleans;
using Microsoft.Azure.Cosmos;
using Orleans.Clustering.Cosmos;
using Orleans.Configuration;
using Orleans.Persistence.Cosmos;
using Silo.AutoPopulation;


var builder = WebApplication.CreateBuilder(args);
builder
    .Host.UseOrleans((ctx, siloBuilder) => 
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
                options.SiloName = ClusterConfig.SiloName;
            })
            .ConfigureEndpoints(siloPort: 11_111, gatewayPort: 30_000)
            .ConfigureServices(services =>
            {
                services.AddHostedService<PopulationService>();
            })
            ;

        if (cosmosDbKey is { } && cosmosDbUri is { })
        {
            siloBuilder.UseCosmosClustering((CosmosClusteringOptions opt) =>
            {
                opt.IsResourceCreationEnabled = true;
                opt.DatabaseName = CosmosDbConfig.CosmosOrleansDbName;
                opt.ClientOptions = new CosmosClientOptions() { ConnectionMode = ConnectionMode.Direct };
                opt.ConfigureCosmosClient(accountEndpoint: cosmosDbUri, authKeyOrResourceToken: cosmosDbKey);
            });
            siloBuilder.AddCosmosGrainStorageAsDefault((CosmosGrainStorageOptions opt) =>
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
            siloBuilder.AddMemoryGrainStorageAsDefault();
        }
        
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddWebAppApplicationInsights("Silo");

// uncomment this if you dont mind hosting grains in the dashboard
builder.Services.DontHostGrainsHere();

var app = builder.Build();
app.MapControllers();



// app.MapGet("/", () => Results.Ok("Silo"));

app.Run();
