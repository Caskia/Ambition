using Ambition.Core.Extensions;
using Ambition.Core.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace Ambition.Core.Scheduler
{
    public class HttpRequestTask : BaseRequestTask, IRequestTask
    {
        #region Fields

        public string Body { get; set; }

        public TimeSpan CycleRequestTimeSpan { get; set; } = TimeSpan.Zero;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

        public new string Identity => Encrypt.Md5Encrypt($"{HttpMethod},{Uri},{Body}");

        public bool IsCycleRequest { get; set; } = false;

        public string UserAgent { get; set; } = "Hicoin-Spider";

        #endregion Fields

        #region Ctor

        public HttpRequestTask(string url) : this(url, HttpMethod.Get)
        {
        }

        public HttpRequestTask(string url, HttpMethod httpMethod)
            : base(url)
        {
            if (Uri.Scheme != "http" && Uri.Scheme != "https")
            {
                Status = RequestTaskStatus.Failed;
                return;
            }

            this.HttpMethod = httpMethod;
        }

        #endregion Ctor

        #region Methods

        public virtual HttpRequestTask AddBody(string body, Encoding encoding = default(Encoding))
        {
            if (body.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (this.HttpMethod != HttpMethod.Post || this.HttpMethod != HttpMethod.Put)
            {
                throw new Exception($"HttpMethod[{this.HttpMethod}] Uri[{this.Uri}] can not add body!");
            }

            if (encoding == default(Encoding))
            {
                encoding = Encoding.UTF8;
            }

            Body = body;
            Encoding = encoding;

            return this;
        }

        public virtual HttpRequestTask AddFormBody(dynamic obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(obj);
            }

            AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var json = JsonConvert.SerializeObject(obj);
            Dictionary<string, string> dicJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var paramPairs = dicJson.Select(p => $"{p.Key}={p.Value}");
            var body = string.Join("&", paramPairs);
            AddBody(body);

            return this;
        }

        public virtual HttpRequestTask AddHeader(string key, string value)
        {
            if (!key.IsNullOrEmpty() && !value.IsNullOrEmpty())
            {
                if (!Headers.ContainsKey(key))
                {
                    Headers.Add(key, value);
                }
                else
                {
                    Headers[key] = value;
                }
            }

            return this;
        }

        public virtual HttpRequestTask AddHeader(string headerContent)
        {
            if (headerContent.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(headerContent));
            }
            var headers = headerContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var header in headers)
            {
                var keyValue = header.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length == 2)
                {
                    AddHeader(keyValue[0].Trim(), keyValue[1].Trim());
                }
            }

            return this;
        }

        public virtual HttpRequestTask AddHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                AddHeader(header.Key, header.Value);
            }

            return this;
        }

        public virtual HttpRequestTask AddJsonBody(dynamic obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(obj);
            }

            AddHeader("Content-Type", "application/json");

            string json = JsonConvert.SerializeObject(obj);
            AddBody(json);

            return this;
        }

        public virtual HttpRequestTask AddPostFormBody(dynamic obj)
        {
            this.HttpMethod = HttpMethod.Post;
            AddFormBody(obj);
            return this;
        }

        public virtual HttpRequestTask AddPostJsonBody(dynamic obj)
        {
            this.HttpMethod = HttpMethod.Post;
            AddJsonBody(obj);
            return this;
        }

        public virtual HttpRequestTask AddPutFormBody(dynamic obj)
        {
            this.HttpMethod = HttpMethod.Put;
            AddFormBody(obj);
            return this;
        }

        public virtual HttpRequestTask AddPutJsonBody(dynamic obj)
        {
            this.HttpMethod = HttpMethod.Put;
            AddJsonBody(obj);
            return this;
        }

        public virtual HttpRequestTask SetUserAgent(string userAgent)
        {
            this.UserAgent = userAgent;
            return this;
        }

        public virtual HttpRequestTask UseCycleRequest(TimeSpan timeSpan = default(TimeSpan))
        {
            if (timeSpan == default(TimeSpan))
            {
                timeSpan = TimeSpan.FromMinutes(1);
            }
            IsCycleRequest = true;
            CycleRequestTimeSpan = timeSpan;
            return this;
        }

        #endregion Methods
    }
}