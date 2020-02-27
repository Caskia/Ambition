using Ambition.Scheduler;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class GDaxRequestTask : WebSocketRequestTask
    {
        public GDaxRequestTask() : base("wss://ws-feed.gdax.com")
        {
        }

        public override IDictionary<string, string> Commands => new Dictionary<string, string>()
        {
            { "GDAX-subscribe-ticker", JsonConvert.SerializeObject(new { type = "subscribe", channels = new string[] { "ticker" }, product_ids = new string[] { "BTC-USD" } }) }
        };
    }
}