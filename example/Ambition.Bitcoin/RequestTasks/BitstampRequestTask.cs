using Ambition.Scheduler;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class BitstampRequestTask : WebSocketRequestTask
    {
        public BitstampRequestTask() :
            base
            (
                "wss://ws.pusherapp.com/app/de504dc5763aeef9ff52?protocol=5&client=Hicoin-Spider&version=0.0.1",
                new Dictionary<string, string>()
                {
                    { "Bitstamp-subscribe-trades", JsonConvert.SerializeObject(new { @event = "pusher:subscribe", data = new { channel = "live_trades" } })}
                }
            )
        {
        }
    }
}