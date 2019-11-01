using Ambition.Core.Scheduler;

namespace Ambition.Core.Fetcher
{
    public interface IFetcherProvider
    {
        IFetcher GetTaskFetcher(IRequestTask requestTask);
    }
}