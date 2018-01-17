using Ambition.Core.Scheduler;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Core.Fetcher
{
    public interface IFetcher
    {
        event EventHandler<FetchResult> FetchedEventHandler;

        void AddAfterFetchCompleteHandler(IAfterFetchCompleteHandler afterFetchCompleteHandler);

        Task FetchAsync(IRequestTask requestTask, CancellationToken cancellationToken);
    }
}