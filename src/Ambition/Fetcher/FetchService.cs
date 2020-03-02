using Ambition.Pipeline;
using Ambition.Processor;
using Ambition.Scheduler;
using Ambition.Timing;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Fetcher
{
    public class FetchService : IFetchService
    {
        private readonly IFetcherProvider _fetcherProvider;
        private readonly IFetchResultProcessorProvider _fetchResultProcessorProvider;
        private readonly ILogger _logger;
        private readonly IPipelineProvider _pipelineProvider;

        public FetchService
            (
                IFetcherProvider fetcherProvider,
                IFetchResultProcessorProvider fetchResultProcessorProvider,
                IPipelineProvider pipelineProvider,
                ILoggerFactory loggerFactory
            )
        {
            _fetcherProvider = fetcherProvider;
            _fetchResultProcessorProvider = fetchResultProcessorProvider;
            _pipelineProvider = pipelineProvider;
            _logger = loggerFactory.CreateLogger<FetchService>();
        }

        public async Task FetchAsync(IRequestTask requestTask, CancellationToken cancellationToken)
        {
            while (true)
            {
                var result = await TryFetchAsync(requestTask, cancellationToken);
                if (result)
                {
                    break;
                }

                requestTask.LastTryTime = Clock.Now;
                requestTask.TryCount += 1;
                requestTask.NextTryTime = requestTask.CalculateNextTryTime();
                _logger.LogInformation($"try to connect to uri[{requestTask.Uri}] at {requestTask.NextTryTime.ToString("yyyyMMddHHmmss")}");

                var now = Clock.Now;
                var delayTimeSpan = new TimeSpan();
                if (requestTask.NextTryTime > now)
                {
                    delayTimeSpan = requestTask.NextTryTime - now;
                }
                await Task.Delay(delayTimeSpan, cancellationToken);

                _logger.LogInformation($"try to connect to uri[{requestTask.Uri}]");
            }
        }

        protected virtual void OnReceivedContent(IRequestTask requestTask, string content)
        {
            requestTask.TryCount = 0;

            var fetchResult = new FetchResult(content, requestTask);
            var requestTaskType = requestTask.GetType();

            var processors = _fetchResultProcessorProvider.GetFetchResultProcessors(requestTaskType);
            foreach (var processor in processors)
            {
                processor.Process(fetchResult);
            }

            var pipelines = _pipelineProvider.GetPipelines(requestTaskType);
            foreach (var pipeline in pipelines)
            {
                Task.Factory.StartNew(async () =>
                {
                    await pipeline.HandleAsync(fetchResult);
                });
            }
        }

        protected async Task<bool> TryFetchAsync(IRequestTask requestTask, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation($"cancel fetch resource!");
                    return true;
                }

                await _fetcherProvider.GetFetcher(requestTask.GetType()).FetchAsync(requestTask, OnReceivedContent, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogInformation($"uri[{requestTask.Uri}] canceled!", ex);
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogInformation($"uri[{requestTask.Uri}] canceled!", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"uri[{requestTask.Uri}] connect error!", ex);

                return false;
            }

            return true;
        }
    }
}