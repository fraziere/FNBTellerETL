using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    internal abstract class CommandValidator
    {
        internal string Command { get; private set; }

        internal CommandValidator(string command)
        {
            //enforce case insensitivity
            this.Command = command.ToLower();
        }

        private static readonly Regex alphaNumeric = new Regex("^[a-zA-Z0-9]*$");
        internal void IsValid()
        {
            if (String.IsNullOrWhiteSpace(Command))
                throw new ETLModelException("Command/Mode commands are required");
            if (Command.Contains(" "))
                throw new ETLModelException($"Command/Mode commands cannot contain whitespace. Failed for: ({Command})");
            if (alphaNumeric.IsMatch(Command) == false)
                throw new ETLModelException($"Command/Mode commands must be alphanumeric only. Failed for: ({Command})");
        }
    }
}
