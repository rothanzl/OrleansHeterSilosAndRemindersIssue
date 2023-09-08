﻿using Microsoft.Extensions.Logging;
using Orleans.Placement;
using Orleans.Runtime;
using Orleans.Runtime.Placement;

namespace Abstractions.OrleansCommon
{
    public class DontPlaceMeOnTheDashboardSiloDirector : IPlacementDirector
    {
        public IGrainFactory GrainFactory { get; set; }
        public IManagementGrain ManagementGrain { get; set; }
        private readonly ILogger<DontPlaceMeOnTheDashboardSiloDirector> _logger;

        public DontPlaceMeOnTheDashboardSiloDirector(IGrainFactory grainFactory, ILogger<DontPlaceMeOnTheDashboardSiloDirector> logger)
        {
            GrainFactory = grainFactory;
            _logger = logger;
            ManagementGrain = GrainFactory.GetGrain<IManagementGrain>(0);
        }

        public async Task<SiloAddress> OnAddActivation(PlacementStrategy strategy, PlacementTarget target, IPlacementContext context)
        {
            var activeSilos = await ManagementGrain.GetDetailedHosts(onlyActive: true);
            _logger.LogInformation($"Silos[{activeSilos.Length}] wiht names {string.Join(", ", activeSilos.Select(s => s.SiloName))}");
            _logger.LogInformation($"Silos[{activeSilos.Length}] wiht names {string.Join(", ", activeSilos.Select(s => s.RoleName))}");
            var silos = activeSilos.Where(x => !x.SiloName.ToLower().Contains("dashboard")).Select(x => x.SiloAddress).ToArray();
            return silos[new Random().Next(0, silos.Length)];
        }
    }

    [Serializable]
    public sealed class DontPlaceMeOnTheDashboardStrategy : PlacementStrategy
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DontPlaceMeOnTheDashboardAttribute : PlacementAttribute
    {
        public DontPlaceMeOnTheDashboardAttribute() :
            base(new DontPlaceMeOnTheDashboardStrategy())
        {
        }
    }
}