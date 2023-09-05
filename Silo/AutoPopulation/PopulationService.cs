using System.Reflection.Metadata.Ecma335;
using Silo.TestGrains;

namespace Silo.AutoPopulation;

public class PopulationService : BackgroundService, IAsyncDisposable
{
    public PopulationService(IGrainFactory grainFactory, ILogger<PopulationService> logger, Orleans.Runtime.Silo silo)
    {
        _grainFactory = grainFactory;
        _logger = logger;
        PopulationTask = Task.CompletedTask;
        PopulationCycleNumber = 0;
        SiloAddress = silo.SiloAddress.ToString();
    }

    private string SiloAddress { get; }
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<PopulationService> _logger;
    private Task PopulationTask { get; set; }
    private readonly object _mutex = new();
    private bool Populate { get; set; }
    private long PopulationCycleNumber { get; set; }
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!PopulationTask.IsCompleted)
            return;

        Populate = true;
        PopulationTask = Task.Run(PopulateLoop);
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

                    var pk = $"{SiloAddress}-{PopulationCycleNumber.ToString()}";
                    PopulationCycleNumber += 1;
                    
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

        try
        {
            await PopulationTask;
        }catch{/* */}
    }
}