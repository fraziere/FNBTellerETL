using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    internal class ETLChainCommand
    {
        int _counter;
        internal string Command { get; private set; }

        internal string Name { get; private set; }

        internal string Description { get; private set; }

        internal Dictionary<int, ETLCommand> CommandsByOrder { get; private set; }
        internal ETLChainCommand(string command, string name, string description)
        {
            this.Command = command.ToLower();
            this.Name = name;
            this.Description = description;
            CommandsByOrder = new Dictionary<int, ETLCommand>();
            _counter = 1;
        }

        internal void AddCommandWithMode(string command, string name, string description, Func<ETLJobIn, ETLJobOut> method, ETLMode mode, bool continueOnError)
        {
            var cmd = new ETLCommand(command, name, description);
            cmd.IsValid();
            cmd.AddMode(mode);
            cmd.AddDelegate(method);
            CommandsByOrder.Add(_counter, cmd);
            _counter++;

        }
    }
}
