using Duplex.Core.Contracts;
using Duplex.Core.Services;
using Duplex.Core.Common;

namespace Duplex.Extensions
{
    public static class DuplexServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IPrizeService, PrizeService>();

            return services;
        }
    }
}
