using RPDB.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB
{
    public class ApplicationSettings
    {
        private static ApplicationSettings _instance = new ApplicationSettings();
        public ILogger Logger { get; set; }

        public bool AutoClose { get; set; }

        public static ApplicationSettings Current
        {
            get { return _instance; }
        }

        private ApplicationSettings() { }

    }
}
