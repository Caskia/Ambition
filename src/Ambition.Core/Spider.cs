using Ambition.Core.Fetcher;
using Ambition.Core.Infrastructure;
using Ambition.Core.Pipeline;
using Ambition.Core.Processor;
using Ambition.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Core
{
    public class Spider : ISpider
    {
        #region Properties

        private readonly IServiceProvider _serviceProvider;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private IList<IFetchResultProcessor> _fetchResultProcessors = new List<IFetchResultProcessor>();
        private IList<IPipeline> _pipelines = new List<IPipeline>();
        private IScheduler _scheduler = new InMemoryScheduler();
        private int _threadNum = 1;
        public string Identity { get; set; }

        public IScheduler Scheduler
        {
            get => _scheduler;
            set
            {
                _scheduler = value;
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

        #endregion Properties

        #region Ctor

        public Spider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Spider(string identity, IList<IFetchResultProcessor> fetchResultProcessors, IList<IPipeline> pipelines)
        {
            Identity = identity;
            AddFetchResultProcessors(fetchResultProcessors);
            AddPipelines(pipelines);
        }

        #endregion Ctor

        #region Static Methods

        public static Spider Create()
        {
            return Create(null, null);
        }

        public static Spider Create(IList<IFetchResultProcessor> fetchResultProcessors)
        {
            return Create(fetchResultProcessors, null);
        }

        public static Spider Create(IList<IFetchResultProcessor> fetchResultProcessors, IList<IPipeline> pipelines)
        {
            return Create(Guid.NewGuid().ToString("N"), fetchResultProcessors, pipelines);
        }

        public static Spider Create(string identity, IList<IFetchResultProcessor> fetchResultProcessors, IList<IPipeline> pipelines)
        {
            return new Spider(identity, fetchResultProcessors, pipelines);
        }

        #endregion Static Methods

        #region Methods

        public Spider AddFetchResultProcessor(IFetchResultProcessor fetchResultProcessor)
        {
            if (fetchResultProcessor != null)
            {
                _fetchResultProcessors.Add(fetchResultProcessor);
            }
            return this;
        }

        public Spider AddFetchResultProcessors(IList<IFetchResultProcessor> fetchResultProcessors)
        {
            if (fetchResultProcessors != null)
            {
                foreach (var fetchResultProcessor in fetchResultProcessors)
                {
                    AddFetchResultProcessor(fetchResultProcessor);
                }
            }
            return this;
        }

        public Spider AddPipeline(IPipeline pipeline)
        {
            if (pipeline != null)
            {
                _pipelines.Add(pipeline);
            }
            return this;
        }

        public Spider AddPipelines(IList<IPipeline> pipelines)
        {
            if (pipelines != null)
            {
                foreach (var pipeline in pipelines)
                {
                    AddPipeline(pipeline);
                }
            }
            return this;
        }

        public Spider AddPipelines(params IPipeline[] pipelines)
        {
            if (pipelines != null)
            {
                foreach (var pipeline in pipelines)
                {
                    AddPipeline(pipeline);
                }
            }
            return this;
        }

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
                LogHelper.Logger.Warn("Spider is running, can not run again!");
                return;
            }

            Status = SpiderStatus.Running;

            Parallel.For(0, ThreadNum, new ParallelOptions() { MaxDegreeOfParallelism = ThreadNum }, async i =>
            {
                while (Status == SpiderStatus.Running)
                {
                    var request = await Scheduler.PollAsync();

                    UsingFetcher(request, fetcher =>
                    {
                        fetcher.FetchAsync(request, _cancellationTokenSource.Token).Wait();
                    });
                }
            });
        }

        public void Stop()
        {
            if (Status == SpiderStatus.Stopped)
            {
                LogHelper.Logger.Warn("Spider is stopped, can not stop again!");
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

        protected void UsingFetcher(IRequestTask request, Action<IFetcher> action)
        {
            IFetcher fetcher = null;

            if (request is WebSocketRequestTask)
            {
                fetcher = new WebSocketFetcher(request.FetchResultProcessors);
            }
            else if (request is SocketIORequestTask)
            {
                fetcher = new SocketIOFetcher(request.FetchResultProcessors);
            }
            else if (request is HttpRequestTask)
            {
                fetcher = new HttpFetcher(request.FetchResultProcessors);
            }
            else
            {
                LogHelper.Logger.Error($"RequestTask[{request.Identity}][{request.GetType().Name}] not support!");
                return;
            }

            foreach (var fetchResultProcessor in _fetchResultProcessors)
            {
                fetcher.AddAfterFetchCompleteHandler(fetchResultProcessor);
            }

            foreach (var pipeline in _pipelines)
            {
                fetcher.FetchedEventHandler += async (sender, fetchResult) =>
                {
                    await pipeline.HandleAsync(fetchResult);
                };
            }

            action(fetcher);
        }

        #endregion Methods
    }
}