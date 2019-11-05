using Ambition.Fetcher;
using System.Threading.Tasks;

namespace Ambition.Pipeline
{
    public interface IPipeline
    {
        Task HandleAsync(FetchResult fetchResult);
    }
}