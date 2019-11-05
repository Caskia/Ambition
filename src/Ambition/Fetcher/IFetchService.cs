using Ambition.Scheduler;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Fetcher
{
    public interface IFetchService
    {
        Task FetchAsync(IRequestTask requestTask, CancellationToken cancellationToken);
    }
}