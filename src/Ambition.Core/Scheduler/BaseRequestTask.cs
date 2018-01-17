using Ambition.Core.Timing;
using Newtonsoft.Json;
using System;
using Ambition.Core.Processor;
using System.Collections.Generic;
using Ambition.Core.Extensions;
using Ambition.Core.Fetcher;

namespace Ambition.Core.Scheduler
{
    public abstract class BaseRequestTask : IRequestTask
    {
        #region Properties

        public int DefaultFirstWaitDuration { get; set; } = 30;

        public double DefaultWaitFactor { get; set; } = 2.0;

        public IList<IFetchResultProcessor> FetchResultProcessors { get; set; } = new List<IFetchResultProcessor>();

        public string Identity { get; }

        public DateTime? LastTryTime { get; set; }

        public DateTime NextTryTime { get; set; }

        public int Priority { get; set; }

        public ContentType ResultContentType { get; set; } = ContentType.Json;

        public RequestTaskStatus Status { get; set; } = RequestTaskStatus.Wait;

        public short TryCount { get; set; }

        public Uri Uri { get; set; }

        #endregion Properties

        #region Ctor

        public BaseRequestTask(string url)
        {
            if (url.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(url));
            }
            if (Uri.TryCreate(url.TrimEnd('#'), UriKind.RelativeOrAbsolute, out var tmp))
            {
                Uri = tmp;
            }
            else
            {
                Status = RequestTaskStatus.Failed;
                return;
            }
        }

        #endregion Ctor

        #region Methods

        public virtual IRequestTask AddDefaultFetchResultProcessor(Action<FetchResult> resultHanler = null)
        {
            var defaultFetchResultProcessor = new DefaultFetchResultProcessor();
            if (resultHanler != null)
            {
                defaultFetchResultProcessor.SetResultHandler(resultHanler);
            }
            return AddFetchResultProcessor(defaultFetchResultProcessor);
        }

        public virtual IRequestTask AddFetchResultProcessor(IFetchResultProcessor fetchResultProcessor)
        {
            FetchResultProcessors.Add(fetchResultProcessor);
            return this;
        }

        public virtual DateTime? CalculateNextTryTime()
        {
            var nextWaitDuration = DefaultFirstWaitDuration * (Math.Pow(DefaultWaitFactor, TryCount - 1));
            var nextTryDate = LastTryTime.HasValue
                ? LastTryTime.Value.AddSeconds(nextWaitDuration)
                : Clock.Now.AddSeconds(nextWaitDuration);

            return nextTryDate;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion Methods
    }
}