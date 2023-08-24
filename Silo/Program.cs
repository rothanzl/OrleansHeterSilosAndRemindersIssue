using Common.Orleans;
using Microsoft.Azure.Cosmos;
using Orleans.Clustering.Cosmos;
using Orleans.Configuration;


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
                options.SiloName = "Silo";
            })
            .ConfigureEndpoints(siloPort: 11_111, gatewayPort: 30_000);

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


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddWebAppApplicationInsights("Silo");

// uncomment this if you dont mind hosting grains in the dashboard
builder.Services.DontHostGrainsHere();

var app = builder.Build();
app.MapControllers();



// app.MapGet("/", () => Results.Ok("Silo"));

app.Run();
