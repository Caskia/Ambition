using Ambition.Fetcher;
using Ambition.Pipeline;
using Ambition.Processor;
using Ambition.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;

namespace Ambition.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAmbition(this IServiceCollection services)
        {
            services.AddSingleton<InMemoryScheduler>();
            services.AddSingleton<IFetchService, FetchService>();
            services.AddSingleton<HttpFetcher>();
            services.AddHttpClient("default", c => { })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var httpClientHandler = new HttpClientHandler();
                        httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
                        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                        httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        return httpClientHandler;
                    });
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