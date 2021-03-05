using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Data
{
    public class AppSetting
    {
        [Key]
        public AppSettingEnum Id { get; set; }
        public string Value { get; set; }
    }
}
