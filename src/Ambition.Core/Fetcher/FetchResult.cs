using Ambition.Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Net;

namespace Ambition.Core.Fetcher
{
    public class FetchResult
    {
        public FetchResult(string content, IRequestTask requestTask) : this(content, requestTask, HttpStatusCode.OK)
        {
        }

        public FetchResult(string content, IRequestTask requestTask, HttpStatusCode httpStatus)
        {
            Content = content;
            RequestTask = requestTask;
            HttpStatus = httpStatus;
            Uri = requestTask.Uri;
        }

        public string Content { get; set; }

        public ContentType ContentType { get; set; }

        public object DeserializedObject { get; set; }

        public IList<Exception> Exceptions { get; set; } = new List<Exception>();

        public HttpStatusCode HttpStatus { get; set; }

        public IRequestTask RequestTask { get; set; }

        public Uri Uri { get; set; }
    }
}