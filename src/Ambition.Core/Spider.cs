using Ambition.Core.Fetcher;
using Ambition.Core.Infrastructure;
using Ambition.Core.Scheduler;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Core
{
    public class Spider : ISpider
    {
        #region Properties

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private IScheduler _scheduler;
        private int _threadNum = 1;
        public string Identity { get; set; }

        public IScheduler Scheduler
        {
            set
            {
                _scheduler = value;
            }
            get
            {
                if (_scheduler == null)
                {
                    _scheduler = _serviceProvider.GetService(typeof(InMemoryScheduler)) as IScheduler;
                }

                return _scheduler;
            }
        }

        public SpiderStatus Status { get; set; } = SpiderStatus.Init;

        public int ThreadNum
        {
            get => _threadNum;
            set
            {
                CheckIfRunning();

                if (value <= 0)
                {
                    throw new ArgumentException("ThreadNum should be more than one!");
                }

                _threadNum = value;
            }
        }

        private IFetchService FetchService => _serviceProvider.GetService(typeof(IFetchService)) as IFetchService;

        #endregion Properties

        #region Ctor

        public Spider(IServiceProvider serviceProvider)
            : this("default", serviceProvider)
        {
        }

        public Spider(string identity, IServiceProvider serviceProvider)
        {
            Identity = identity;
            _serviceProvider = serviceProvider;

            _logger = new Log4NetLoggerFactory().Create(nameof(Spider));
        }

        #endregion Ctor

        #region Static Methods

        public static Spider Create(IServiceProvider serviceProvider)
        {
            return new Spider(serviceProvider);
        }

        public static Spider Create(string identity, IServiceProvider serviceProvider)
        {
            return new Spider(identity, serviceProvider);
        }

        #endregion Static Methods

        #region Methods

        public async Task<Spider> AddTaskAsync(IRequestTask requestTask)
        {
            await Scheduler.PushAsync(requestTask);
            return this;
        }

        public async Task<Spider> AddTasksAsync(IList<IRequestTask> requestTasks)
        {
            foreach (var requestTask in requestTasks)
            {
                await AddTaskAsync(requestTask);
            }
            return this;
        }

        public async Task<Spider> AddTasksAsync(params IRequestTask[] requestTasks)
        {
            foreach (var requestTask in requestTasks)
            {
                await AddTaskAsync(requestTask);
            }
            return this;
        }

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (Status == SpiderStatus.Running)
            {
                _logger.Warn("Spider is running, can not run again!");
                return;
            }

            Status = SpiderStatus.Running;

            Parallel.For(0, ThreadNum, new ParallelOptions() { MaxDegreeOfParallelism = ThreadNum }, async i =>
            {
                while (Status == SpiderStatus.Running)
                {
                    var request = await Scheduler.PollAsync();

                    await FetchService.FetchAsync(request, _cancellationTokenSource.Token);
                }
            });
        }

        public void Stop()
        {
            if (Status == SpiderStatus.Stopped)
            {
                _logger.Warn("Spider is stopped, can not stop again!");
                return;
            }

            _cancellationTokenSource.Cancel();
            Status = SpiderStatus.Stopped;

            Thread.Sleep(2000);

            _cancellationTokenSource.Dispose();
        }

        protected void CheckIfRunning()
        {
            if (Status == SpiderStatus.Running)
            {
                throw new Exception("Spider is running.");
            }
        }

        #endregion Methods
    }
}