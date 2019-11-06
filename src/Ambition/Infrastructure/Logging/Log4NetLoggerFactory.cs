using Ambition.Extensions;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Xml;

namespace Ambition.Infrastructure.Logging
{
    public class Log4NetLoggerFactory
    {
        internal const string DefaultConfigFileName = "log4net.config";
        private readonly ILoggerRepository _loggerRepository;

        public Log4NetLoggerFactory()
            : this(DefaultConfigFileName)
        {
        }

        public Log4NetLoggerFactory(string configFileName)
        {
            _loggerRepository = LogManager.CreateRepository(
                typeof(Log4NetLoggerFactory).GetAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy)
            );

            var log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead(configFileName));
            XmlConfigurator.Configure(_loggerRepository, log4NetConfig["log4net"]);
        }

        public ILog Create(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return LogManager.GetLogger(_loggerRepository.Name, name);
        }
    }
}