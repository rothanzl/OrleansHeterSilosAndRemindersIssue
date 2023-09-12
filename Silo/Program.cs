using Abstractions;
using Abstractions.OrleansCommon;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orleans.Clustering.Cosmos;
using Orleans.Configuration;
using Orleans.Persistence.Cosmos;
using Orleans.Reminders.Cosmos;
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
            
            siloBuilder.UseCosmosReminderService((CosmosReminderTableOptions opt) =>
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
            siloBuilder.UseInMemoryReminderService();
        }
        
    });

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddWebAppApplicationInsights("Silo");

// uncomment this if you dont mind hosting grains in the dashboard
builder.Services.DontHostGrainsHere();

var app = builder.Build();
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = (context, report) =>
    {
        string json = System.Text.Json.JsonSerializer.Serialize(
            new
            {
                Status = report.Status.ToString(),
                Duration = report.TotalDuration,
                Info = report.Entries
                    .Select(e =>
                        new
                        {
                            Key = e.Key,
                            Description = e.Value.Description,
                            Duration = e.Value.Duration,
                            Status = Enum.GetName(
                                typeof(HealthStatus),
                                e.Value.Status),
                            Error = e.Value.Exception?.Message,
                            Data = e.Value.Data
                        })
                    .ToList()
            });
        return context.Response.WriteAsync(json);
    }
});


// app.MapGet("/", () => Results.Ok("Silo"));

app.Run();
