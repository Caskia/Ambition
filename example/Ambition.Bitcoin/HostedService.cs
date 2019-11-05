using Ambition.Bitcoin.RequestTasks;
using Ambition;
using Ambition.Fetcher;
using Ambition.Pipeline;
using Ambition.Processor;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Bitcoin
{
    public class HostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Spider _spider;

        public HostedService(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
            _spider = Spider.Create(_serviceProvider);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //websocket
            _spider
            .AddOrUpdateFetcher<GDaxRequestTask>(typeof(WebSocketFetcher))
            .AddOrUpdateFetchResultProcessors<GDaxRequestTask>(new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            })
            .AddOrUpdatePipelines<GDaxRequestTask>(new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            })
            .AddOrUpdateFetcher<BitfinexRequestTask>(typeof(WebSocketFetcher))
            .AddOrUpdateFetchResultProcessors<BitfinexRequestTask>(new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            })
            .AddOrUpdatePipelines<BitfinexRequestTask>(new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            })
            .AddOrUpdateFetcher<GeminiRequestTask>(typeof(WebSocketFetcher))
            .AddOrUpdateFetchResultProcessors<GeminiRequestTask>(new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            })
            .AddOrUpdatePipelines<GeminiRequestTask>(new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            })
            .AddOrUpdateFetcher<BitstampRequestTask>(typeof(WebSocketFetcher))
            .AddOrUpdateFetchResultProcessors<BitstampRequestTask>(new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            })
            .AddOrUpdatePipelines<BitstampRequestTask>(new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            })
            //socket io
            .AddOrUpdateFetcher<CryptoCompareRequestTask>(typeof(SocketIOFetcher))
            .AddOrUpdateFetchResultProcessors<CryptoCompareRequestTask>(new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            })
            .AddOrUpdatePipelines<CryptoCompareRequestTask>(new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            })
            //http
            .AddOrUpdateFetcher<GDaxHttpRequestTask>(typeof(HttpFetcher))
            .AddOrUpdateFetchResultProcessors<GDaxHttpRequestTask>(new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            })
            .AddOrUpdatePipelines<GDaxHttpRequestTask>(new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            });

            await _spider.AddTaskAsync(new GDaxRequestTask());
            await _spider.AddTaskAsync(new BitfinexRequestTask());
            await _spider.AddTaskAsync(new GeminiRequestTask());
            await _spider.AddTaskAsync(new BitstampRequestTask());
            await _spider.AddTaskAsync(new CryptoCompareRequestTask());
            await _spider.AddTaskAsync(new GDaxHttpRequestTask());

            await _spider.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _spider.StopAsync();
        }
    }
}