using log4net;
using Microsoft.Extensions.Logging;
using System;

namespace Ambition.Infrastructure.Logging
{
    public static class Log4NetExtensions
    {
        public static void Log(this ILog log, LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Critical:
                    log.Fatal(message);
                    break;

                case LogLevel.Error:
                    log.Error(message);
                    break;

                case LogLevel.Warning:
                    log.Warn(message);
                    break;

                case LogLevel.Information:
                    log.Info(message);
                    break;

                case LogLevel.Debug:
                case LogLevel.Trace:
                    log.Debug(message);
                    break;

                default:
                    return;
            }
        }

        public static void Log(this ILog log, LogLevel level, string message, Exception exception)
        {
            switch (level)
            {
                case LogLevel.Critical:
                    log.Fatal(message, exception);
                    break;

                case LogLevel.Error:
                    log.Error(message, exception);
                    break;

                case LogLevel.Warning:
                    log.Warn(message, exception);
                    break;

                case LogLevel.Information:
                    log.Info(message, exception);
                    break;

                case LogLevel.Debug:
                case LogLevel.Trace:
                    log.Debug(message, exception);
                    break;

                default:
                    return;
            }
        }

        public static void Log(this ILog log, LogLevel level, Func<string> messageFactory)
        {
            switch (level)
            {
                case LogLevel.Critical:
                    log.Fatal(messageFactory);
                    break;

                case LogLevel.Error:
                    log.Error(messageFactory);
                    break;

                case LogLevel.Warning:
                    log.Warn(messageFactory);
                    break;

                case LogLevel.Information:
                    log.Info(messageFactory);
                    break;

                case LogLevel.Debug:
                case LogLevel.Trace:
                    log.Debug(messageFactory);
                    break;

                default:
                    return;
            }
        }
    }
}