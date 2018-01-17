using Ambition.Core.Fetcher;
using Newtonsoft.Json;

namespace Ambition.Core.Processor
{
    public class DefaultFetchResultProcessor : BaseFetchResultProcessor
    {
        protected override void Handle(FetchResult fetchResult)
        {
            if (fetchResult.ContentType == ContentType.Json)
            {
                fetchResult.DeserializedObject = JsonConvert.DeserializeObject(fetchResult.Content);
            }
        }
    }
}