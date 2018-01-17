using System.Threading.Tasks;

namespace Ambition.Core.Scheduler
{
    public interface IScheduler
    {
        Task<IRequestTask> PollAsync();

        Task PushAsync(IRequestTask requestTask);
    }
}