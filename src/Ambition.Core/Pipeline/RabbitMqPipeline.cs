using Ambition.Core.Fetcher;
using Ambition.Core.Infrastructure;
using System.Threading.Tasks;

namespace Ambition.Core.Pipeline
{
    public class RabbitMqPipeline : IPipeline
    {
        public async Task HandleAsync(FetchResult fetchResult)
        {
            if (fetchResult.DeserializedObject != null)
            {
                await MasstransitBus.Instance.BusControl.Publish(fetchResult.DeserializedObject);
            }
        }
    }
}