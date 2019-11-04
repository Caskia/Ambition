using Ambition.Core.Infrastructure;
using Ambition.Core.Scheduler;
using Ambition.Core.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Core.Fetcher
{
    public class WebSocketFetcher : IFetcher
    {
        public WebSocketFetcher()
        {
        }

        public async Task FetchAsync(IRequestTask requestTask, Action<IRequestTask, string> onReceived, CancellationToken cancellationToken)
        {
            if (!TypeUtils.IsClassAssignableFrom(requestTask.GetType(), typeof(WebSocketRequestTask)))
            {
                throw new ArgumentException($"{nameof(requestTask)} is not websocket task");
            }

            var webSocketRequestTask = requestTask as WebSocketRequestTask;

            var client = new ClientWebSocket();
            await client.ConnectAsync(requestTask.Uri, cancellationToken);

            //send command
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Task.Delay(1000, cancellationToken);
                    foreach (var command in webSocketRequestTask.Commands)
                    {
                        var commandJson = JsonConvert.SerializeObject(command.Value);
                        var commandBytes = Encoding.UTF8.GetBytes(commandJson);
                        await client.SendAsync(new ArraySegment<byte>(commandBytes), WebSocketMessageType.Text, true, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Logger.Error($"Websocket[{requestTask.Uri}] send command error!", ex);
                }
            });

            //receive message
            try
            {
                while (client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    var buffer = new ArraySegment<byte>(new Byte[1024 * 16]);
                    string serializedMessage = null;
                    WebSocketReceiveResult result = null;
                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            result = await client.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            serializedMessage = await reader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        onReceived(requestTask, serializedMessage);
                    }
                }

                client.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error($"receive Websocket[{requestTask.Uri}] data error!", ex);

                client.Dispose();
                throw ex;
            }
        }
    }
}