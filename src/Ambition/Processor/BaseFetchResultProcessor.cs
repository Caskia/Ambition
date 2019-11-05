using Ambition.Fetcher;
using Newtonsoft.Json.Linq;
using System;

namespace Ambition.Processor
{
    public abstract class BaseFetchResultProcessor : IFetchResultProcessor
    {
        public void Process(FetchResult fetchResult)
        {
            if (fetchResult == null)
            {
                throw new ArgumentNullException(nameof(fetchResult));
            }

            TryDetectContentType(fetchResult);

            Handle(fetchResult);
        }

        protected abstract void Handle(FetchResult fetchResult);

        private void TryDetectContentType(FetchResult fetchResult)
        {
            if (fetchResult != null && fetchResult.Exceptions.Count == 0)
            {
                if (fetchResult.RequestTask.ResultContentType == ContentType.Auto)
                {
                    try
                    {
                        JToken.Parse(fetchResult.Content);
                        fetchResult.ContentType = ContentType.Json;
                    }
                    catch
                    {
                        fetchResult.ContentType = ContentType.Html;
                    }
                }
                else
                {
                    fetchResult.ContentType = fetchResult.RequestTask.ResultContentType;
                }
            }
        }
    }
}