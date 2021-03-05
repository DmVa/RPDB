using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Logs
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
        void LogError(string message, Exception ex);
    }
}
