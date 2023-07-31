﻿using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Orleans.Runtime.Placement;

namespace Common.Orleans
{
    public static class DontHostGrainsHereServiceCollectionExtensions
    {
        public static IServiceCollection DontHostGrainsHere(this IServiceCollection services)
        {
            services.AddSingletonNamedService<PlacementStrategy, DontPlaceMeOnTheDashboardStrategy>(nameof(DontPlaceMeOnTheDashboardSiloDirector));

            services.AddSingletonKeyedService<Type, IPlacementDirector, DontPlaceMeOnTheDashboardSiloDirector>(
                    typeof(DontPlaceMeOnTheDashboardStrategy));

            return services;
        }
    }
}