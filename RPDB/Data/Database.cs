using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Data
{
    public class Database
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public List<RegisteredScript> Scripts { get; set; }
    }
}
