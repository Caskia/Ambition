using Ambition.Scheduler;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class BtcToolsRequestTask : WebSocketRequestTask
    {
        public BtcToolsRequestTask() :
            base
            (
                "wss://ws.btctools.io/socketcluster/",
                new Dictionary<string, string>()
                {
                    { "HandShake", JsonConvert.SerializeObject(new { @event = "#handshake", data = new { authToken = "" }, cid = 1 }) },
                    { "SubscribeTrades", JsonConvert.SerializeObject(new { @event = "#subscribe", data = new { channel = "bitmex_xbtusd_trades" }, cid = 2 }) }
                },
                new Dictionary<string, string>()
                {
                    { "HeartBeat", "#2" }
                }
            )
        {
        }
    }
}