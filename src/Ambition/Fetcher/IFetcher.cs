using Ambition.Scheduler;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Fetcher
{
    public interface IFetcher
    {
        Task FetchAsync(IRequestTask requestTask, Action<IRequestTask, string> onReceivedContent, CancellationToken cancellationToken);
    }
}