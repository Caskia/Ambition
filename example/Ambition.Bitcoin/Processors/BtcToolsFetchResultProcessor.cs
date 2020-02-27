using Ambition.Processor;
using Newtonsoft.Json;

namespace Ambition.Bitcoin.Processors
{
    public class BtcToolsFetchResultProcessor : DefaultFetchResultProcessor
    {
        public BtcToolsFetchResultProcessor()
        {
            SetResultHandler((fetchResult) =>
            {
                if (fetchResult.ContentType == ContentType.Json && fetchResult.Content.StartsWith("{") && fetchResult.Content.EndsWith("}"))
                {
                    fetchResult.DeserializedObject = JsonConvert.DeserializeObject(fetchResult.Content);
                }
            });
        }
    }
}