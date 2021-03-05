using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Logs
{
    public enum LogLevel
    {
        /// <summary>
        /// Not visibile messages in production.
        /// </summary>
        Debug,
        /// <summary>
        /// Visible messages in production, but not errors.
        /// </summary>
        Info,
        /// <summary>
        /// Errors, visible in production.
        /// </summary>
        Error
    }
}
