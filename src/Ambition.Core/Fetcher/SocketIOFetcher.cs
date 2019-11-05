using Ambition.Core.Infrastructure;
using Ambition.Core.Scheduler;
using Ambition.Core.Utils;
using Castle.Core.Logging;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Core.Fetcher
{
    public class SocketIOFetcher : IFetcher
    {
        private readonly ILogger _logger;

        public SocketIOFetcher()
        {
            _logger = new Log4NetLoggerFactory().Create(nameof(SocketIOFetcher));
        }

        public Task FetchAsync(IRequestTask requestTask, Action<IRequestTask, string> onReceived, CancellationToken cancellationToken)
        {
            if (!TypeUtils.IsClassAssignableFrom(requestTask.GetType(), typeof(SocketIORequestTask)))
            {
                throw new ArgumentException($"{nameof(requestTask)} is not socket-io task");
            }

            var socketIORequestTask = requestTask as SocketIORequestTask;

            var client = IO.Socket(socketIORequestTask.Uri, null);
            client.Open();
            try
            {
                bool isDisconnect = false;
                client.On(Socket.EVENT_DISCONNECT, message =>
                {
                    client.Disconnect();
                    isDisconnect = true;
                });

                client.On(Socket.EVENT_ERROR, message =>
                {
                    client.Disconnect();
                    isDisconnect = true;
                });

                foreach (var @event in socketIORequestTask.Events)
                {
                    client.On(@event, message =>
                    {
                        onReceived(requestTask, message.ToString());
                    });
                }

                foreach (var command in socketIORequestTask.Commands)
                {
                    var commandJson = JObject.FromObject(command.Value);
                    client.Emit(command.Key, commandJson);
                }

                while (!isDisconnect)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        client.Disconnect();
                        client.Close();
                        isDisconnect = true;
                        return Task.CompletedTask;
                    }

                    Thread.Sleep(1000);
                }

                throw new Exception("socket-io disconnect!");
            }
            catch (Exception ex)
            {
                _logger.Error($"receive socket-io[{requestTask.Uri}] data error!", ex);

                client.Disconnect();
                client.Close();
                throw ex;
            }
        }
    }
}