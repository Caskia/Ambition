using Ambition.Core.Scheduler;

namespace Ambition.Bitcoin.RequestTasks
{
    public class GeminiRequestTask : WebSocketRequestTask
    {
        public GeminiRequestTask() : base("wss://api.gemini.com/v1/marketdata/btcusd")
        {
        }
    }
}