using Ambition.Extensions;
using Ambition.Infrastructure;
using Ambition.Scheduler;
using Ambition.Utils;
using Castle.Core.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Fetcher
{
    public class HttpFetcher : IFetcher
    {
        private readonly ILogger _logger;

        public HttpFetcher()
        {
            _logger = new Log4NetLoggerFactory().Create(nameof(HttpFetcher));
        }

        public async Task FetchAsync(IRequestTask requestTask, Action<IRequestTask, string> onReceived, CancellationToken cancellationToken)
        {
            if (!TypeUtils.IsClassAssignableFrom(requestTask.GetType(), typeof(HttpRequestTask)))
            {
                throw new ArgumentException($"{nameof(requestTask)} is not http task");
            }

            var httpRequestTask = requestTask as HttpRequestTask;

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            var httpClient = new HttpClient(httpClientHandler);

            HttpRequestMessage httpRequestMessage;
            HttpResponseMessage httpResponse;
            try
            {
                while (true && !cancellationToken.IsCancellationRequested)
                {
                    httpRequestMessage = GenerateHttpRequestMessage(httpRequestTask);
                    httpResponse = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

                    httpResponse.EnsureSuccessStatusCode();

                    var content = ReadContent(httpResponse, httpRequestTask);

                    onReceived(requestTask, content);

                    if (!httpRequestTask.IsCycleRequest)
                    {
                        httpRequestTask.Status = RequestTaskStatus.Success;
                        break;
                    }

                    await Task.Delay(httpRequestTask.CycleRequestTimeSpan, cancellationToken);
                }

                httpClient.Dispose();
            }
            catch (HttpRequestException hre)
            {
                httpClient.Dispose();

                _logger.Error($"HttpMethod[{httpRequestTask.HttpMethod}] Uri[{requestTask.Uri}]  response status error!", hre);
                throw hre;
            }
            catch (Exception ex)
            {
                httpClient.Dispose();

                _logger.Error($"receive HttpMethod[{httpRequestTask.HttpMethod}] Uri[{requestTask.Uri}] data error!", ex);
                throw ex;
            }
        }

        private HttpRequestMessage GenerateHttpRequestMessage(HttpRequestTask requestTask)
        {
            if (requestTask == null)
            {
                throw new ArgumentNullException(nameof(requestTask));
            }

            var httpRequestMessage = new HttpRequestMessage(requestTask.HttpMethod, requestTask.Uri);

            if (!requestTask.UserAgent.IsNullOrEmpty())
            {
                httpRequestMessage.Headers.Add("User-Agent", requestTask.UserAgent);
            }

            foreach (var header in requestTask.Headers)
            {
                if (!header.Key.IsNullOrEmpty() && !header.Value.IsNullOrEmpty() && header.Key != "Content-Type" && header.Key != "User-Agent")
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            if (requestTask.HttpMethod == HttpMethod.Post || requestTask.HttpMethod == HttpMethod.Put)
            {
                var body = requestTask.Encoding.GetBytes(requestTask.Body);
                httpRequestMessage.Content = new StreamContent(new MemoryStream(body));

                if (requestTask.Headers.ContainsKey("Content-Type"))
                {
                    httpRequestMessage.Content.Headers.Add("Content-Type", requestTask.Headers["Content-Type"]);
                }

                if (requestTask.Headers.ContainsKey("X-Requested-With") && requestTask.Headers["X-Requested-With"] == "NULL")
                {
                    httpRequestMessage.Content.Headers.Remove("X-Requested-With");
                }
                else
                {
                    if (!httpRequestMessage.Content.Headers.Contains("X-Requested-With") && !httpRequestMessage.Headers.Contains("X-Requested-With"))
                    {
                        httpRequestMessage.Content.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    }
                }
            }

            return httpRequestMessage;
        }

        private byte[] PreventCutOff(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x00)
                {
                    bytes[i] = 32;
                }
            }
            return bytes;
        }

        private string ReadContent(HttpResponseMessage response, HttpRequestTask requestTask)
        {
            byte[] contentBytes = response.Content.ReadAsByteArrayAsync().Result;
            contentBytes = PreventCutOff(contentBytes);
            if (requestTask.Encoding == null)
            {
                var charSet = response.Content.Headers.ContentType?.CharSet;
                var htmlCharset = EncodingExtensions.GetEncoding(charSet, contentBytes);
                return htmlCharset.GetString(contentBytes, 0, contentBytes.Length);
            }
            else
            {
                return requestTask.Encoding.GetString(contentBytes, 0, contentBytes.Length);
            }
        }
    }
}