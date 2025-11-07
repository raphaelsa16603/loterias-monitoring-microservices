using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Loterias.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedDependencies(this IServiceCollection services)
        {
            services.AddLogging(cfg => cfg.AddConsole());
            return services;
        }
    }
}
