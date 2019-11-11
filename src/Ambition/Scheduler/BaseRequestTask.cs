using Ambition.Extensions;
using Ambition.Timing;
using Newtonsoft.Json;
using System;

namespace Ambition.Scheduler
{
    public abstract class BaseRequestTask : IRequestTask
    {
        #region Properties

        public int DefaultFirstWaitDuration { get; set; } = 30;

        public double DefaultWaitFactor { get; set; } = 2.0;

        public virtual string Identity { get; }

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