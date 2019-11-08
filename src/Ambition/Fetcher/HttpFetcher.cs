using Ambition.Extensions;
using Ambition.Scheduler;
using Ambition.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Fetcher
{
    public class HttpFetcher : IFetcher
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public HttpFetcher(
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory
            )
        {
            _httpClientFactory = httpClientFactory;
            _logger = loggerFactory.CreateLogger<HttpFetcher>();
        }

        public virtual async Task FetchAsync(IRequestTask requestTask, Action<IRequestTask, string> onReceivedContent, CancellationToken cancellationToken)
        {
            if (!TypeUtils.IsClassAssignableFrom(requestTask.GetType(), typeof(HttpRequestTask)))
            {
                throw new ArgumentException($"{nameof(requestTask)} is not http task");
            }

            var httpClient = _httpClientFactory.CreateClient("default");
            var httpRequestTask = requestTask as HttpRequestTask;
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

                    onReceivedContent(requestTask, content);

                    if (!httpRequestTask.IsCycleRequest)
                    {
                        httpRequestTask.Status = RequestTaskStatus.Success;
                        break;
                    }

                    await Task.Delay(httpRequestTask.CycleRequestTimeSpan, cancellationToken);
                }
            }
            catch (HttpRequestException hre)
            {
                _logger.LogError($"HttpMethod[{httpRequestTask.HttpMethod}] Uri[{requestTask.Uri}]  response status error!", hre);
                throw hre;
            }
            catch (Exception ex)
            {
                _logger.LogError($"receive HttpMethod[{httpRequestTask.HttpMethod}] Uri[{requestTask.Uri}] data error!", ex);
                throw ex;
            }
        }

        private HttpRequestMessage GenerateHttpRequestMessage(HttpRequestTask requestTask)
        {
            if (requestTask == null)
            {
                throw new ArgumentNullException(nameof(requestTask));
            }

            var httpRequestMessage = new HttpRequestMessage(requestTask.HttpMethod, requestTask.RequestUrl);

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