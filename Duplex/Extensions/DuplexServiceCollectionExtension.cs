using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Contracts.Administration;
using Duplex.Core.Services;
using Duplex.Core.Services.Administration;

namespace Duplex.Extensions
{
    public static class DuplexServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRankService, RankService>();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IPrizeService, PrizeService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IBetService, BetService>();
            services.AddScoped<IRiotService, RiotService>();

            return services;
        }
    }
}
