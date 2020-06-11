using Ambition.Scheduler;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class CryptoCompareRequestTask : SocketIORequestTask
    {
        public CryptoCompareRequestTask() :
            base
            (
                "https://streamer.cryptocompare.com/",
                new Dictionary<string, dynamic>()
                {
                    { "SubAdd",  new { subs = new[] { "5~CCCAGG~BTC~USD", "5~CCCAGG~ETH~USD", "5~CCCAGG~EOS~USD", "5~CCCAGG~BCH~USD", "5~CCCAGG~LTC~USD" } } }
                }, new HashSet<string>()
                {
                    "m"
                }
            )
        {
            ResultContentType = ContentType.Html;
        }
    }
}