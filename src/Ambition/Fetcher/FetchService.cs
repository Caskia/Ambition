using Ambition.Infrastructure;
using Ambition.Pipeline;
using Ambition.Processor;
using Ambition.Scheduler;
using Ambition.Timing;
using Castle.Core.Logging;
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
                IPipelineProvider pipelineProvider
            )
        {
            _fetcherProvider = fetcherProvider;
            _fetchResultProcessorProvider = fetchResultProcessorProvider;
            _pipelineProvider = pipelineProvider;
            _logger = new Log4NetLoggerFactory().Create(nameof(FetchService));
        }

        public async Task FetchAsync(IRequestTask requestTask, CancellationToken cancellationToken)
        {
            requestTask.TryCount += 1;
            requestTask.LastTryTime = Clock.Now;

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Info($"cancel fetch resource!");
                    return;
                }

                await _fetcherProvider.GetFetcher(requestTask.GetType()).FetchAsync(requestTask, OnReceivedContent, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                _logger.Info($"uri[{requestTask.Uri.ToString()}] canceled!", ex);
            }
            catch (ObjectDisposedException ex)
            {
                _logger.Info($"uri[{requestTask.Uri.ToString()}] canceled!", ex);
            }
            catch (Exception ex)
            {
                _logger.Error($"uri[{requestTask.Uri.ToString()}] connect error!", ex);

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
                    _logger.Info($"try to connect to uri[{requestTask.Uri.ToString()}] at {nextTryTime.Value.ToString("yyyyMMddHHmmss")}");
                    await Task.Delay((int)(requestTask.NextTryTime - Clock.Now).TotalMilliseconds, cancellationToken);

                    requestTask.LastTryTime = requestTask.NextTryTime;

                    _logger.Info($"try to connect to uri[{requestTask.Uri.ToString()}]");
                    await FetchAsync(requestTask, cancellationToken);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.Info($"uri[{requestTask.Uri.ToString()}] canceled!", ex);
                }
                catch (ObjectDisposedException ex)
                {
                    _logger.Info($"uri[{requestTask.Uri.ToString()}] canceled!", ex);
                }
                catch (Exception ex)
                {
                    _logger.Error($"uri[{requestTask.Uri.ToString()}] connect error!", ex);
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