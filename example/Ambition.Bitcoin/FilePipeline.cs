using Ambition.Fetcher;
using Ambition.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ambition.Bitcoin
{
    public class FilePipeline : IPipeline
    {
        private List<string> contents = new List<string>();
        private string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}/contents.txt";
        private object objLock = new object();

        public FilePipeline()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    WriteTask();
                    Thread.Sleep(200);
                }
            });
        }

        public Task HandleAsync(FetchResult fetchResult)
        {
            lock (objLock)
            {
                contents.Add(fetchResult.Content);
            }

            return Task.CompletedTask;
        }

        private void WriteTask()
        {
            lock (objLock)
            {
                File.AppendAllLines(filePath, contents);
            }
        }
    }
}