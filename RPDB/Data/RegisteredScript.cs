using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Data
{
    public class RegisteredScript
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime? FileTime { get; set; }
        public int? FileSize { get; set; }
        public bool Executed { get; set; }
        public bool Ignore { get; set; }
        public DateTime? ExecutedTime { get; set; }
        public string LastExecutionError { get; set; }
        public int DatabaseId { get; set; }
        public Database Database { get; set; }
    }
}
