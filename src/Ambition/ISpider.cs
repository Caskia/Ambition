using System.Threading;
using System.Threading.Tasks;

namespace Ambition
{
    public interface ISpider
    {
        Task ContinueAsync();

        Task PauseAsync();

        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync();
    }
}