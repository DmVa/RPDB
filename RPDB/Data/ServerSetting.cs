using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Data
{
    public class ServerSetting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsWindowsAuth { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
