using Ambition.Core.Scheduler;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class GDaxRequestTask : WebSocketRequestTask
    {
        public GDaxRequestTask() : base("wss://ws-feed.gdax.com")
        {
        }

        public override IDictionary<string, dynamic> Commands => new Dictionary<string, dynamic>()
        {
            { "GDAX-subscribe-ticker", new { type = "subscribe", channels = new string[] { "ticker" }, product_ids = new string[] { "BTC-USD" } } }
        };
    }
}