using Ambition.Core.Scheduler;
using System;

namespace Ambition.Bitcoin.RequestTasks
{
    public class GDaxHttpRequestTask : HttpRequestTask
    {
        public GDaxHttpRequestTask() : base("https://api.gdax.com/products/btc-usd/stats")
        {
            UseCycleRequest(TimeSpan.FromSeconds(5));
        }
    }
}