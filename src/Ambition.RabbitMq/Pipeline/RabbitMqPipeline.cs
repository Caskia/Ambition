using Ambition.Fetcher;
using Ambition.RabbitMq.MasstransitBus;
using System.Threading.Tasks;

namespace Ambition.Pipeline
{
    public class RabbitMqPipeline : IPipeline
    {
        private readonly MasstransitBus _masstransitBus;

        public RabbitMqPipeline(MasstransitBus masstransitBus)
        {
            _masstransitBus = masstransitBus;
        }

        public async Task HandleAsync(FetchResult fetchResult)
        {
            if (fetchResult.DeserializedObject != null)
            {
                await _masstransitBus.BusControl.Publish(fetchResult.DeserializedObject);
            }
        }
    }
}