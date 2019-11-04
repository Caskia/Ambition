using Ambition.Core.Scheduler;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class BitfinexRequestTask : WebSocketRequestTask
    {
        public BitfinexRequestTask() : base("wss://api.bitfinex.com/ws")
        {
        }

        public override IDictionary<string, dynamic> Commands => new Dictionary<string, dynamic>()
        {
            { "Bitfinex-subscribe-ticker", new {  @event = "subscribe", channel = "Ticker", symbol = "BTCUSD" } }
        };
    }
}