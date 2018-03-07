using Ambition.Core.Pipeline;
using Ambition.Core.Processor;
using Ambition.Core.Scheduler;
using System;

namespace Ambition.Bitcoin
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //websocket request
            var gdaxCommand = new { type = "subscribe", channels = new string[] { "ticker" }, product_ids = new string[] { "BTC-USD" } };
            var gdaxRequest = new WebSocketRequestTask("wss://ws-feed.gdax.com")
                .AddCommand("GDAX-subscribe-ticker", gdaxCommand)
                .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            var bitfinexCommand = new { @event = "subscribe", channel = "Ticker", symbol = "BTCUSD" };
            var bitfinexRequest = new WebSocketRequestTask("wss://api.bitfinex.com/ws")
                .AddCommand("Bitfinex-subscribe-ticker", bitfinexCommand)
                .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            var geminiRequest = new WebSocketRequestTask("wss://api.gemini.com/v1/marketdata/btcusd")
                .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            var bitstampCommand = new { @event = "pusher:subscribe", data = new { channel = "live_trades" } };
            var bitstampRequest = new WebSocketRequestTask("wss://ws.pusherapp.com/app/de504dc5763aeef9ff52?protocol=5&client=Hicoin-Spider&version=0.0.1")
                .AddCommand("Bitstamp-subscribe-trades", bitstampCommand)
                .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            //socket-io request
            var cryptocompareEvent = "m";
            var cryptocompareCommand = new { subs = new[] { "5~CCCAGG~BTC~USD" } };
            var cryptocompareRequest = new SocketIORequestTask("https://streamer.cryptocompare.com/")
            {
                ResultContentType = Core.ContentType.Html
            }
                .AddEvent(cryptocompareEvent)
                .AddCommand("SubAdd", cryptocompareCommand)
                .AddDefaultFetchResultProcessor();

            //http request
            var gdaxHttpRequest = new HttpRequestTask("https://api.gdax.com/products/btc-usd/stats")
                .UseCycleRequest(TimeSpan.FromSeconds(5))
                .AddFetchResultProcessor(new DefaultFetchResultProcessor());

            var spider = Core.Spider.Create()
                .AddPipeline(new ConsolePipeline())
            .AddTasksAsync(cryptocompareRequest).Result;
            //.AddTasksAsync(bitstampRequest, gdaxRequest, bitfinexRequest, geminiRequest, cryptocompareRequest, gdaxHttpRequest).Result;

            spider.ThreadNum = 100;
            spider.Start();

            System.Console.WriteLine("complete");
            System.Console.ReadKey();
        }
    }
}