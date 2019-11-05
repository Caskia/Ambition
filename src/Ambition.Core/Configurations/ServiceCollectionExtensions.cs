using Ambition.Core.Fetcher;
using Ambition.Core.Pipeline;
using Ambition.Core.Processor;
using Ambition.Core.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Ambition.Core.Configurations
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
            services.AddSingleton<RabbitMqPipeline>();
            services.AddSingleton<IPipelineProvider, DefaultPipelineProvider>();

            return services;
        }
    }
}