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
        private readonly IFetcherProvider _fetcherProvider;
        private readonly IFetchResultProcessorProvider _fetchResultProcessorProvider;
        private readonly IPipelineProvider _pipelineProvider;
        private readonly IServiceProvider _serviceProvider;

        public HostedService(
            IFetcherProvider fetcherProvider,
            IFetchResultProcessorProvider fetchResultProcessorProvider,
            IPipelineProvider pipelineProvider,
            IServiceProvider serviceProvider
            )
        {
            _fetcherProvider = fetcherProvider;
            _fetchResultProcessorProvider = fetchResultProcessorProvider;
            _pipelineProvider = pipelineProvider;
            _serviceProvider = serviceProvider;

            Map();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var spider = Spider.Create(_serviceProvider);

            await spider.AddTaskAsync(new GDaxRequestTask());
            await spider.AddTaskAsync(new BitfinexRequestTask());
            await spider.AddTaskAsync(new GeminiRequestTask());
            await spider.AddTaskAsync(new BitstampRequestTask());

            //await spider.AddTaskAsync(new CryptoCompareRequestTask());

            await spider.AddTaskAsync(new GDaxHttpRequestTask());

            spider.ThreadNum = 100;
            spider.Start();

            ////websocket request
            //var gdaxCommand = new { type = "subscribe", channels = new string[] { "ticker" }, product_ids = new string[] { "BTC-USD" } };
            //var gdaxRequest = new WebSocketRequestTask("wss://ws-feed.gdax.com")
            //    .AddCommand("GDAX-subscribe-ticker", gdaxCommand)
            //    .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            //var bitfinexCommand = new { @event = "subscribe", channel = "Ticker", symbol = "BTCUSD" };
            //var bitfinexRequest = new WebSocketRequestTask("wss://api.bitfinex.com/ws")
            //    .AddCommand("Bitfinex-subscribe-ticker", bitfinexCommand)
            //    .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            //var geminiRequest = new WebSocketRequestTask("wss://api.gemini.com/v1/marketdata/btcusd")
            //    .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            //var bitstampCommand = new { @event = "pusher:subscribe", data = new { channel = "live_trades" } };
            //var bitstampRequest = new WebSocketRequestTask("wss://ws.pusherapp.com/app/de504dc5763aeef9ff52?protocol=5&client=Hicoin-Spider&version=0.0.1")
            //    .AddCommand("Bitstamp-subscribe-trades", bitstampCommand)
            //    .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            ////socket-io request
            //var cryptocompareEvent = "m";
            //var cryptocompareCommand = new { subs = new[] { "5~CCCAGG~BTC~USD", "5~CCCAGG~ETH~USD", "5~CCCAGG~EOS~USD", "5~CCCAGG~BCH~USD", "5~CCCAGG~LTC~USD" } };
            //var cryptocompareRequest = new SocketIORequestTask("https://streamer.cryptocompare.com/")
            //{
            //    ResultContentType = Core.ContentType.Html
            //}
            //    .AddEvent(cryptocompareEvent)
            //    .AddCommand("SubAdd", cryptocompareCommand)
            //    .AddDefaultFetchResultProcessor();

            ////http request
            //var gdaxHttpRequest = new HttpRequestTask("https://api.gdax.com/products/btc-usd/stats")
            //    .UseCycleRequest(TimeSpan.FromSeconds(5))
            //    .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            //var spider = Core.Spider.Create()
            //    .AddPipeline(new FilePipeline())
            //    .AddPipeline(new ConsolePipeline())
            //.AddTasksAsync(cryptocompareRequest).Result;
            ////.AddTasksAsync(bitstampRequest, gdaxRequest, bitfinexRequest, geminiRequest, cryptocompareRequest, gdaxHttpRequest).Result;

            //spider.ThreadNum = 100;
            //spider.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void Map()
        {
            //websocket
            _fetcherProvider.AddOrUpdateFetcher(typeof(GDaxRequestTask), typeof(WebSocketFetcher));
            _fetchResultProcessorProvider.AddOrUpdateFetchResultProcessors(typeof(GDaxRequestTask), new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            });
            _pipelineProvider.AddOrUpdatePipelines(typeof(GDaxRequestTask), new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            });

            _fetcherProvider.AddOrUpdateFetcher(typeof(BitfinexRequestTask), typeof(WebSocketFetcher));
            _fetchResultProcessorProvider.AddOrUpdateFetchResultProcessors(typeof(BitfinexRequestTask), new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            });
            _pipelineProvider.AddOrUpdatePipelines(typeof(BitfinexRequestTask), new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            });

            _fetcherProvider.AddOrUpdateFetcher(typeof(GeminiRequestTask), typeof(WebSocketFetcher));
            _fetchResultProcessorProvider.AddOrUpdateFetchResultProcessors(typeof(GeminiRequestTask), new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            });
            _pipelineProvider.AddOrUpdatePipelines(typeof(GeminiRequestTask), new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            });

            _fetcherProvider.AddOrUpdateFetcher(typeof(BitstampRequestTask), typeof(WebSocketFetcher));
            _fetchResultProcessorProvider.AddOrUpdateFetchResultProcessors(typeof(BitstampRequestTask), new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            });
            _pipelineProvider.AddOrUpdatePipelines(typeof(BitstampRequestTask), new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            });

            //socket io
            _fetcherProvider.AddOrUpdateFetcher(typeof(CryptoCompareRequestTask), typeof(SocketIOFetcher));
            _fetchResultProcessorProvider.AddOrUpdateFetchResultProcessors(typeof(CryptoCompareRequestTask), new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            });
            _pipelineProvider.AddOrUpdatePipelines(typeof(CryptoCompareRequestTask), new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            });

            //http
            _fetcherProvider.AddOrUpdateFetcher(typeof(GDaxHttpRequestTask), typeof(HttpFetcher));
            _fetchResultProcessorProvider.AddOrUpdateFetchResultProcessors(typeof(GDaxHttpRequestTask), new List<Type>()
            {
                typeof(DefaultFetchResultProcessor)
            });
            _pipelineProvider.AddOrUpdatePipelines(typeof(GDaxHttpRequestTask), new List<Type>()
            {
                typeof(FilePipeline),
                typeof(ConsolePipeline)
            });
        }
    }
}