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
            requestTask.TryCount += 1;
            requestTask.LastTryTime = Clock.Now;

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation($"cancel fetch resource!");
                    return;
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

                await NextTryFetchAsync(requestTask, cancellationToken);
            }
        }

        protected virtual async Task NextTryFetchAsync(IRequestTask requestTask, CancellationToken cancellationToken)
        {
            var nextTryTime = requestTask.CalculateNextTryTime();
            if (nextTryTime.HasValue)
            {
                requestTask.NextTryTime = nextTryTime.Value;

                try
                {
                    _logger.LogInformation($"try to connect to uri[{requestTask.Uri}] at {nextTryTime.Value.ToString("yyyyMMddHHmmss")}");
                    await Task.Delay((int)(requestTask.NextTryTime - Clock.Now).TotalMilliseconds, cancellationToken);

                    requestTask.LastTryTime = requestTask.NextTryTime;

                    _logger.LogInformation($"try to connect to uri[{requestTask.Uri}]");
                    await FetchAsync(requestTask, cancellationToken);
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
                }
            }
        }

        protected virtual void OnReceivedContent(IRequestTask requestTask, string content)
        {
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
    }
}