using Ambition.Scheduler;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class BitfinexRequestTask : WebSocketRequestTask
    {
        public BitfinexRequestTask() : base("wss://api.bitfinex.com/ws")
        {
        }

        public override IDictionary<string, string> Commands => new Dictionary<string, string>()
        {
            { "Bitfinex-subscribe-ticker", JsonConvert.SerializeObject(new {  @event = "subscribe", channel = "Ticker", symbol = "BTCUSD" })}
        };
    }
}