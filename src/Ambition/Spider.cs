using Ambition.Fetcher;
using Ambition.Pipeline;
using Ambition.Processor;
using Ambition.Scheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition
{
    public class Spider : ISpider
    {
        #region Properties

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private IScheduler _scheduler;
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
                    _scheduler = _serviceProvider.GetService<InMemoryScheduler>();
                }

                return _scheduler;
            }
        }

        public SpiderStatus Status { get; set; } = SpiderStatus.Init;

        private IFetcherProvider FetcherProvider => _serviceProvider.GetService<IFetcherProvider>();

        private IFetchResultProcessorProvider FetchResultProcessorProvider => _serviceProvider.GetService<IFetchResultProcessorProvider>();

        private IFetchService FetchService => _serviceProvider.GetService<IFetchService>();

        private IPipelineProvider PipelineProvider => _serviceProvider.GetService<IPipelineProvider>();

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

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<Spider>();
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

        #region Fetcher

        public Spider AddOrUpdateFetcher<TRequestTask>(Type fetcherType) where TRequestTask : IRequestTask
        {
            FetcherProvider.AddOrUpdateFetcher<TRequestTask>(fetcherType);
            return this;
        }

        public Spider DeleteFetcher<TRequestTask>() where TRequestTask : IRequestTask
        {
            FetcherProvider.DeleteFetcher<TRequestTask>();
            return this;
        }

        #endregion Fetcher

        #region FetchResultProcessor

        public Spider AddOrUpdateFetchResultProcessors<TRequestTask>(List<Type> processorTypes) where TRequestTask : IRequestTask
        {
            FetchResultProcessorProvider.AddOrUpdateFetchResultProcessors<TRequestTask>(processorTypes);
            return this;
        }

        public Spider DeleteFetchResultProcessors<TRequestTask>() where TRequestTask : IRequestTask
        {
            FetchResultProcessorProvider.DeleteFetchResultProcessors<TRequestTask>();
            return this;
        }

        #endregion FetchResultProcessor

        #region Pipeline

        public Spider AddOrUpdatePipelines<TRequestTask>(List<Type> pipelineTypes) where TRequestTask : IRequestTask
        {
            PipelineProvider.AddOrUpdatePipelines<TRequestTask>(pipelineTypes);
            return this;
        }

        public Spider DeletePipelines<TRequestTask>() where TRequestTask : IRequestTask
        {
            PipelineProvider.DeletePipelines<TRequestTask>();
            return this;
        }

        #endregion Pipeline

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

        public Task ContinueAsync()
        {
            throw new NotImplementedException();
        }

        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (Status == SpiderStatus.Running)
            {
                _logger.LogWarning("Spider is running, can not run again!");
                return;
            }
            Status = SpiderStatus.Running;

            while (Status == SpiderStatus.Running)
            {
                var request = await Scheduler.PollAsync();

                _ = Task.Factory.StartNew(async r =>
                {
                    var oRequest = r as IRequestTask;
                    await FetchService.FetchAsync(oRequest, cancellationToken);
                }, request);
            }
        }

        public Task StopAsync()
        {
            if (Status == SpiderStatus.Stopped)
            {
                _logger.LogWarning("Spider stopped, can not stop again!");
                return Task.CompletedTask;
            }

            Status = SpiderStatus.Stopped;

            Thread.Sleep(2000);

            return Task.CompletedTask;
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