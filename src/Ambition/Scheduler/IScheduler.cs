using System.Threading.Tasks;

namespace Ambition.Scheduler
{
    public interface IScheduler
    {
        Task<IRequestTask> PollAsync();

        Task PushAsync(IRequestTask requestTask);
    }
}