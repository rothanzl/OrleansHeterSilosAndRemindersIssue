using Orleans.BroadcastChannel;
using Silo.BroadcastChannel;

namespace Silo.AutoPopulation;

public class PopulationService : BackgroundService, IAsyncDisposable
{
    public PopulationService(IGrainFactory grainFactory, ILogger<PopulationService> logger, Orleans.Runtime.Silo silo, IClusterClient clusterClient)
    {
        _grainFactory = grainFactory;
        _logger = logger;
        _provider = clusterClient.GetBroadcastChannelProvider(Constants.NameSpace);
        
        PopulationTasks = Array.Empty<Task>();
        PopulationCycleNumber = 0;
        
        SiloIpAddressIdentifier = Int64.Parse(silo.SiloAddress.Endpoint.Address.ToString().Replace(".",""));
    }
    
    private long SiloIpAddressIdentifier { get; }
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<PopulationService> _logger;
    private readonly IBroadcastChannelProvider _provider;
    private Task[] PopulationTasks { get; set; }
    private readonly object _mutex = new();
    private bool Populate { get; set; }
    private long PopulationCycleNumber { get; set; }
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!PopulationTasks.All(t=> t.IsCompleted))
            return;

        Populate = true;

        int parallelism;
        if (!Int32.TryParse(Environment.GetEnvironmentVariable("POPULATE_PARALLELISM")??"", out parallelism))
            parallelism = 1;
        
        _logger.LogInformation("Population parallelism {P}", parallelism);
        
        
        PopulationTasks = Enumerable
            .Range(0, parallelism)
            .Select(_ => Task.Run(PopulateLoop))
            .ToArray();
    }

    private async Task PopulateLoop()
    {
        ChannelId channelId = ChannelId.Create(Constants.NameSpace, Constants.Key);
        IBroadcastChannelWriter<long[]> channelWriter = _provider.GetChannelWriter<long[]>(channelId);

        // Activate consumer grain before publishing
        await _grainFactory.GetGrain<IConsumerGrain>(Constants.Key).Activate();
        
        long siloId;
        lock (_mutex)
        {
            siloId = SiloIpAddressIdentifier;
        }

        bool populate = true;
        while (populate)
        {
            try
            {
                if (!await ITestConfigGrain.GetInstance(_grainFactory).IsTestEnabled())
                {
                    await Task.Delay(1000);
                }
                else
                {
                    long counter;
                    lock (_mutex)
                    {
                        counter = PopulationCycleNumber;
                        PopulationCycleNumber += 1;
                    }

                    var payload = CreatePayload(siloId, counter);
                    await channelWriter.Publish(payload);
                    await _grainFactory.GetGrain<IStatsGrain>(Constants.Key).SetProducerCounter(counter);
                }
                
                lock (_mutex)
                {
                    populate = Populate;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Population loop error");
                await Task.Delay(1000);
            }
        }
    }

    private static long[] CreatePayload(long firstElement, long otherElemetns, int sizeInKyloBytes = 21)
    {
        var elementSize = sizeof(long);
        var arraySize = (sizeInKyloBytes * 1024) / elementSize;

        return Enumerable.Range(0, arraySize)
            .Select(i => i == 0 ? firstElement : otherElemetns)
            .ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        lock (_mutex)
        {
            Populate = false;
        }

        foreach (Task populationTask in PopulationTasks)
        {
            try
            { 
                await populationTask;
            }
            catch{/* */}
        }
    }
}