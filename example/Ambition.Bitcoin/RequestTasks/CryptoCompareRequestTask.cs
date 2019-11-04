using Ambition.Core.Scheduler;
using System.Collections.Generic;

namespace Ambition.Bitcoin.RequestTasks
{
    public class CryptoCompareRequestTask : SocketIORequestTask
    {
        public CryptoCompareRequestTask() : base("https://streamer.cryptocompare.com/")
        {
            ResultContentType = Core.ContentType.Html;
        }

        public override IDictionary<string, dynamic> Commands => new Dictionary<string, dynamic>()
        {
            { "SubAdd",  new { subs = new[] { "5~CCCAGG~BTC~USD", "5~CCCAGG~ETH~USD", "5~CCCAGG~EOS~USD", "5~CCCAGG~BCH~USD", "5~CCCAGG~LTC~USD" } } }
        };

        public override ISet<string> Events => new HashSet<string>()
        {
            "m"
        };
    }
}