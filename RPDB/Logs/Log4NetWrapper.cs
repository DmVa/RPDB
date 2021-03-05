using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Logs
{
    public class Log4NetWrapper : ILogger
    {
        private readonly log4net.ILog _logger;

        public Log4NetWrapper()
        {
            _logger = LogManager.GetLogger("LOGGER");
            InitLogger();
        }

        private static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
        #region ILogger Members

        public void Log(LogLevel level, string message)
        {
            if (_logger == null)
                return;

            var logger = _logger.Logger;

            logger.Log(logger.GetType(), ToLog4NetLevel(level), message, null);
        }

        public void LogError(string message, Exception ex)
        {
            if (_logger == null)
                return;

            _logger.Error(message, ex);
        }

        #endregion

        private static log4net.Core.Level ToLog4NetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return log4net.Core.Level.Debug;
                case LogLevel.Info:
                    return log4net.Core.Level.Info;
                case LogLevel.Error:
                    return log4net.Core.Level.Error;
            }

            // Null should never happen. Exception will raise in switch if do not any value is valid.
            return null;
        }

        #region ILogger Members




        #endregion
    }
}
