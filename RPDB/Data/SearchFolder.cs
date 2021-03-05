using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Data
{
    public class SearchFolder
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public bool IncludeSubfolders { get; set; }
        public int DatabaseId { get; set; }
        public Database Database { get; set; }
        public int SortOrder { get; set; }
    }
}
