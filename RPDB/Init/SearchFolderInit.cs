using RPDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Init
{
    [NotMapped]
    public class SearchFolderInit: SearchFolder
    {
        public string DatabaseName { get; set; }
    }
}
