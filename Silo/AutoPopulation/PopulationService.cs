using Silo.TestGrains;

namespace Silo.AutoPopulation;

public class PopulationService : BackgroundService, IAsyncDisposable
{
    public PopulationService(IGrainFactory grainFactory, ILogger<PopulationService> logger, Orleans.Runtime.Silo silo)
    {
        _grainFactory = grainFactory;
        _logger = logger;
        PopulationTasks = Array.Empty<Task>();
        PopulationCycleNumber = 0;
        SiloAddress = silo.SiloAddress.ToString();
    }

    private string SiloAddress { get; }
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<PopulationService> _logger;
    private Task[] PopulationTasks { get; set; }
    private readonly object _mutex = new();
    private bool Populate { get; set; }
    private long PopulationCycleNumber { get; set; }
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!PopulationTasks.All(t=> t.IsCompleted))
            return;

        Populate = true;
        PopulationTasks = new Task[]
        {
            Task.Run(PopulateLoop),
            Task.Run(PopulateLoop),
            Task.Run(PopulateLoop),
        };
    }

    private async Task PopulateLoop()
    {
        bool populate = true;
        while (populate)
        {
            try
            {
                if (!await IAutoPopulationConfGrain.GetInstance(_grainFactory).IsPopulationEnabled())
                {
                    await Task.Delay(1000);
                }
                else
                {
                    string pk;
                    lock (_mutex)
                    {
                        pk = $"{SiloAddress}-{PopulationCycleNumber.ToString()}";
                        PopulationCycleNumber += 1;
                    }

                    var count = await _grainFactory
                        .GetGrain<IRecurrentTestGrainInMemory>(pk)
                        .SayHello(true);
                    
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