using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Services
{
    internal class CommandLineActions
    {
        public List<string> Errors = new List<string>();
        public void Run()
        {
            var sc = new ScriptsScanner();
            var scripts = sc.Collect();
            var sr = new ScriptRunner();
            var warnings = new List<string>();  
            var errors = new List<string>();
            if (scripts != null)
            {
                foreach (var script in scripts)
                {
                    try
                    {
                        sr.RunScript(script, script.ExpectedDatabaseId, warnings, errors);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add(ex.Message);
                    }
                    
                }
            }
            Errors = errors;
        }
    }
}
