using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Ambition.Infrastructure.Logging
{
    public class Log4NetLoggerProvider : ILoggerProvider
    {
        private readonly Log4NetLoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers;

        public Log4NetLoggerProvider()
        {
            _loggerFactory = new Log4NetLoggerFactory();
            _loggers = new ConcurrentDictionary<string, Log4NetLogger>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(
                           categoryName,
                           name => new Log4NetLogger(_loggerFactory.Create(name))
                       );
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}