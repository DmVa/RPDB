using RPDB.Data;
using RPDB.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Services
{
    public class ScriptData: BasePropertyChanged
    {
        public ScriptStatus Status { get; set; }
        public string ExpectedDatabaseName
        {
            get
            {
                return Registered?.Database != null ? (Registered.Database == null ? "not loaded": Registered.Database.Name) : FileData.ExpectedDatabaseName;
            }
        }

        public int ExpectedDatabaseId
        {
            get
            {
                return Registered != null ? Registered.DatabaseId : FileData.ExpectedDatabaseId;
            }
        }

        public void RaisePropertyChanged()
        {
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(ExpectedDatabaseName));
            RaisePropertyChanged(nameof(ExpectedDatabaseId));
        }

        public ScriptFile FileData { get; set; }
        public RegisteredScript Registered { get; set; }
    }
}
