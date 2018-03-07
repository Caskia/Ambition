using Ambition.Core.Infrastructure;
using Ambition.Core.Processor;
using Ambition.Core.Scheduler;
using Ambition.Core.Timing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Core.Fetcher
{
    public abstract class BaseFetcher : IFetcher
    {
        #region Fields

        protected IList<IAfterFetchCompleteHandler> afterFetchCompleteHandlers = new List<IAfterFetchCompleteHandler>();

        public event EventHandler<FetchResult> FetchedEventHandler;

        #endregion Fields

        #region Ctors

        public BaseFetcher(IList<IFetchResultProcessor> fetchResultProcessors)
        {
            LoadFetchResultProcessor(fetchResultProcessors);
        }

        #endregion Ctors

        public virtual void AddAfterFetchCompleteHandler(IAfterFetchCompleteHandler afterFetchCompleteHandler)
        {
            afterFetchCompleteHandlers.Add(afterFetchCompleteHandler);
        }

        public virtual async Task FetchAsync(IRequestTask requestTask, CancellationToken cancellationToken)
        {
            requestTask.TryCount += 1;
            requestTask.LastTryTime = Clock.Now;

            try
            {
                await DoFetchAsync(requestTask, OnReceivedContent, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                LogHelper.Logger.Info($"uri[{requestTask.Uri.ToString()}] canceled!", ex);
            }
            catch (ObjectDisposedException ex)
            {
                LogHelper.Logger.Info($"uri[{requestTask.Uri.ToString()}] canceled!", ex);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error($"uri[{requestTask.Uri.ToString()}] connect error!", ex);

                var nextTryTime = requestTask.CalculateNextTryTime();
                if (nextTryTime.HasValue)
                {
                    requestTask.NextTryTime = nextTryTime.Value;

                    LogHelper.Logger.Info($"try to connect to uri[{requestTask.Uri.ToString()}] at {nextTryTime.Value.ToString("yyyyMMddHHmmss")}", ex);
                    await Task.Delay((int)(requestTask.NextTryTime - Clock.Now).TotalMilliseconds);

                    requestTask.LastTryTime = requestTask.NextTryTime;

                    LogHelper.Logger.Info($"try to connect to uri[{requestTask.Uri.ToString()}]");
                    await FetchAsync(requestTask, cancellationToken);
                }
            }
        }

        protected abstract Task DoFetchAsync(IRequestTask requestTask, Action<IRequestTask, string> onReceived, CancellationToken cancellationToken);

        protected virtual void HandleAfterFetchComplete(FetchResult fetchResult)
        {
            foreach (var afterFetchCompleteHandler in afterFetchCompleteHandlers)
            {
                afterFetchCompleteHandler.Process(fetchResult);
            }
        }

        protected virtual void LoadFetchResultProcessor(IList<IFetchResultProcessor> fetchResultProcessors)
        {
            if (fetchResultProcessors != null)
            {
                foreach (var processor in fetchResultProcessors)
                {
                    AddAfterFetchCompleteHandler(processor);
                }
            }
        }

        protected virtual void OnFetched(FetchResult fetchResult)
        {
            FetchedEventHandler?.Invoke(this, fetchResult);
        }

        protected virtual void OnReceivedContent(IRequestTask requestTask, string content)
        {
            var fetchResult = new FetchResult(content, requestTask);

            //process result
            HandleAfterFetchComplete(fetchResult);

            //fetched event handler
            OnFetched(fetchResult);
        }
    }
}