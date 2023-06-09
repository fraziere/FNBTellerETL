using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    internal class ETLCommand : CommandValidator
    {
        internal string Name { get; private set; }

        internal string Description { get; private set; }

        internal List<ETLMode> Modes { get; private set; }
        internal Func<ETLJobIn, ETLJobOut> Method { get; private set; }
        internal bool ContinueOnError { get; private set; } //only use is for chaining commands

        internal ETLCommand(string command, string name, string description, bool continueOnError = true)
            : base(command)
        {
            this.Name = name;
            this.Description = description;
            this.Modes = new List<ETLMode>();
            this.ContinueOnError = continueOnError;
        }

        internal void AddMode(ETLMode mode)
        {
            this.Modes.Add(mode);
        }

        internal void AddDelegate(Func<ETLJobIn, ETLJobOut> method)
        {
            this.Method = method;
        }

        internal void SetContinueOnError(bool continueOnError)
        {
            this.ContinueOnError = continueOnError;
        }
    }
}
