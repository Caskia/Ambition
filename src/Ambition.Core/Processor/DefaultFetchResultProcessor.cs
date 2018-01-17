using Ambition.Core.Fetcher;
using Newtonsoft.Json;
using System;

namespace Ambition.Core.Processor
{
    public class DefaultFetchResultProcessor : BaseFetchResultProcessor
    {
        private Action<FetchResult> _resultHandler = (fetchResult) =>
        {
            if (fetchResult.ContentType == ContentType.Json)
            {
                fetchResult.DeserializedObject = JsonConvert.DeserializeObject(fetchResult.Content);
            }
        };

        public void SetResultHandler(Action<FetchResult> resultHandler)
        {
            _resultHandler = resultHandler;
        }

        protected override void Handle(FetchResult fetchResult)
        {
            _resultHandler?.Invoke(fetchResult);
        }
    }
}