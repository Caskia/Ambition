using Ambition.Core.Scheduler;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class BitstampRequestTask : WebSocketRequestTask
    {
        public BitstampRequestTask() : base("wss://ws.pusherapp.com/app/de504dc5763aeef9ff52?protocol=5&client=Hicoin-Spider&version=0.0.1")
        {
        }

        public override IDictionary<string, dynamic> Commands => new Dictionary<string, dynamic>()
        {
            { "Bitstamp-subscribe-trades", new { @event = "pusher:subscribe", data = new { channel = "live_trades" } } }
        };
    }
}