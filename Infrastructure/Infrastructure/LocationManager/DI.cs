using Application.Common.Services.Location;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.LocationManager
{
    public static class DI
    {
        public static IServiceCollection AddLocationManager(this IServiceCollection services)
        {
            services.AddHttpClient<IAddressResolverService, NominatimAddressResolver>();

            return services;
        }
    }
}
