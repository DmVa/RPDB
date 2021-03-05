using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Services
{
    public class ScriptFile
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public DateTime FileDate { get; set; }
        public string Path { get; set; }
        public string FullFileName { get; set; }
        public int ExpectedDatabaseId { get;  set; }
        public string ExpectedDatabaseName { get;  set; }
    }
}
