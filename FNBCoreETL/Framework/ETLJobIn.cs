using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Framework
{
    public class ETLJobIn
    {
        public bool IsChained { get; private set; }
        public string ChainCommandName { get; private set; }
        public string Command { get; private set; }
        public string CommandName { get; private set; }
        public string ModeCommand { get; private set; }
        public string ModeName { get; private set; }
        public IReadOnlyDictionary<string, string> ArgumentsByName { get; private set; }
        public Func<ETLJobIn, ETLJobOut> Method { get; private set; }
        public bool ContinueOnError { get; private set; }

        internal ETLJobIn(bool isChained, string chainCommandName, 
            string command,string commandName,
            string modeCommand, string modeName,
            Dictionary<string, string> argumentsByName,
            Func<ETLJobIn, ETLJobOut> method,
            bool continueOnError)
        {
            this.IsChained = isChained;
            this.ChainCommandName = chainCommandName;
            this.Command = command;
            this.CommandName = commandName;
            this.ModeCommand = modeCommand;
            this.ModeName = modeName;
            this.ArgumentsByName = argumentsByName;
            this.Method = method;
            this.ContinueOnError = continueOnError;
        }
    }


}
