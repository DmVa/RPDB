using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPDB.Services
{
    internal class CommandLine
    {
        private Dictionary<string, Action> AllCommands = new Dictionary<string, Action>();
        public List<Action> Commands = new List<Action>();
        public CommandLine(string[] args)
        {
            Args = args;
        }

        public void AddCommand(string name, Action action)
        {
            AllCommands.Add(name, action);
            if (Args.Length > 0 && Array.IndexOf(Args, name) >=0 )
            {
                Commands.Add(action);
            }
        }
        public void Execute()
        {
            foreach(var cmd in Commands)
            {
                cmd.Invoke();
            }
        }
        public bool IsValid
        {
            get
            {
                if (Commands.Count == 0)
                    return false;

                return true;
            }
        }
        public string[] Args { get; }
    }
}
