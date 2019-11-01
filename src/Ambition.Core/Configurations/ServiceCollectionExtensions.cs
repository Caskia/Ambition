using Ambition.Core.Fetcher;
using Microsoft.Extensions.DependencyInjection;

namespace Ambition.Core.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSpider(this IServiceCollection services)
        {
            services.AddSingleton<IFetcherProvider, DefaultFetchProvider>();

            return services;
        }
    }
}