using Ambition.Scheduler;
using Ambition.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Fetcher
{
    public class WebSocketFetcher : IFetcher
    {
        private readonly ILogger _logger;

        public WebSocketFetcher(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WebSocketFetcher>();
        }

        public virtual async Task FetchAsync(IRequestTask requestTask, Action<IRequestTask, string> onReceivedContent, CancellationToken cancellationToken)
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
                        var commandBytes = Encoding.UTF8.GetBytes(command.Value);
                        await client.SendAsync(new ArraySegment<byte>(commandBytes), WebSocketMessageType.Text, true, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Websocket[{requestTask.Uri}] send command error!", ex);
                }
            });

            //heartbeat
            if (webSocketRequestTask.HeartBeatCommands != null && webSocketRequestTask.HeartBeatCommands.Count > 0)
            {
                await Task.Factory.StartNew(async () =>
                {
                    while (client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            foreach (var command in webSocketRequestTask.HeartBeatCommands)
                            {
                                var commandBytes = Encoding.UTF8.GetBytes(command.Value);
                                await client.SendAsync(new ArraySegment<byte>(commandBytes), WebSocketMessageType.Text, true, cancellationToken);
                            }

                            await Task.Delay(TimeSpan.FromSeconds(webSocketRequestTask.HeartBeatInterval));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Websocket[{requestTask.Uri}] send heartbeat error!", ex);
                        }
                    }
                });
            }

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
                        onReceivedContent(requestTask, serializedMessage);
                    }
                }

                client.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError($"receive Websocket[{requestTask.Uri}] data error!", ex);

                client.Dispose();
                throw ex;
            }
        }
    }
}