using log4net;
using Microsoft.Extensions.Logging;
using System;

namespace Ambition.Infrastructure.Logging
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;

        public Log4NetLogger(ILog log)
        {
            _log = log;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                case LogLevel.Warning:
                case LogLevel.Information:
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return true;

                case LogLevel.None:
                    return false;

                default:
                    throw new ArgumentException($"{nameof(logLevel)} value is not implemented: " + logLevel);
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            _log.Log(logLevel, message, exception);
        }

        private class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new NullDisposable();

            public void Dispose()
            {
            }
        }
    }
}