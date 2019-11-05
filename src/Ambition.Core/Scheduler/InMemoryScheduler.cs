using Ambition.Core.Infrastructure;
using Ambition.Core.Threading;
using Ambition.Core.Timing;
using Castle.Core.Logging;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Ambition.Core.Scheduler
{
    public class InMemoryScheduler : IScheduler
    {
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly ILogger _logger;
        private ConcurrentDictionary<string, IRequestTask> tasks = new ConcurrentDictionary<string, IRequestTask>();

        public InMemoryScheduler()
        {
            _logger = new Log4NetLoggerFactory().Create(nameof(InMemoryScheduler));
        }

        public async Task<IRequestTask> PollAsync()
        {
            var requestTask = await GetNextWaitingTask();

            while (requestTask == null)
            {
                _logger.Debug("cannot get task wait 1 second to try again.");
                await Task.Delay(1000);
                requestTask = await GetNextWaitingTask();
            }

            return requestTask;
        }

        public async Task PushAsync(IRequestTask requestTask)
        {
            using (await _asyncLock.LockAsync())
            {
                if (!IsDuplicate(requestTask))
                {
                    tasks.TryAdd(requestTask.Identity, requestTask);
                }
            }
        }

        private async Task<IRequestTask> GetNextWaitingTask()
        {
            using (await _asyncLock.LockAsync())
            {
                var task = tasks.Values
                       .Where(t => t.NextTryTime <= Clock.Now && t.Status == RequestTaskStatus.Wait)
                       .OrderBy(t => t.Priority)
                       .ThenBy(t => t.TryCount)
                       .ThenBy(t => t.NextTryTime)
                       .FirstOrDefault();

                if (task != null)
                {
                    task.Status = RequestTaskStatus.Active;
                }

                return task;
            }
        }

        private bool IsDuplicate(IRequestTask requestTask)
        {
            return tasks.ContainsKey(requestTask.Identity);
        }
    }
}