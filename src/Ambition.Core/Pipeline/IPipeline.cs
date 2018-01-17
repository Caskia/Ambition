using Ambition.Core.Fetcher;
using System.Threading.Tasks;

namespace Ambition.Core.Pipeline
{
    public interface IPipeline
    {
        Task HandleAsync(FetchResult fetchResult);
    }
}