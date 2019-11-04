using Ambition.Core.Scheduler;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Core.Fetcher
{
    public interface IFetchService
    {
        Task FetchAsync(IRequestTask requestTask, CancellationToken cancellationToken);
    }
}