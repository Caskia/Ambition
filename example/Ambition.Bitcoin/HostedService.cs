using Ambition.Bitcoin.RequestTasks;
using Ambition.Core;
using Ambition.Core.Fetcher;
using Ambition.Core.Pipeline;
using Ambition.Core.Processor;
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

        public HostedService(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var spider = Spider.Create(_serviceProvider);

            //websocket
            spider
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

            await spider.AddTaskAsync(new GDaxRequestTask());
            await spider.AddTaskAsync(new BitfinexRequestTask());
            await spider.AddTaskAsync(new GeminiRequestTask());
            await spider.AddTaskAsync(new BitstampRequestTask());
            await spider.AddTaskAsync(new CryptoCompareRequestTask());
            await spider.AddTaskAsync(new GDaxHttpRequestTask());

            spider.ThreadNum = 100;
            spider.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}