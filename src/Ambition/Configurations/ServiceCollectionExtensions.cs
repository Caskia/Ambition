using Ambition.Fetcher;
using Ambition.Pipeline;
using Ambition.Processor;
using Ambition.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Ambition.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmbition(this IServiceCollection services)
        {
            services.AddSingleton<InMemoryScheduler>();

            services.AddSingleton<IFetchService, FetchService>();
            services.AddSingleton<HttpFetcher>();
            services.AddSingleton<SocketIOFetcher>();
            services.AddSingleton<WebSocketFetcher>();
            services.AddSingleton<IFetcherProvider, DefaultFetchProvider>();

            services.AddSingleton<DefaultFetchResultProcessor>();
            services.AddSingleton<IFetchResultProcessorProvider, DefaultFetchResultProcessorProvider>();

            services.AddSingleton<ConsolePipeline>();
            services.AddSingleton<IPipelineProvider, DefaultPipelineProvider>();

            return services;
        }
    }
}