using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    internal class ETLMode : CommandValidator
    {
        int _counter;

        internal string Name { get; private set; }

        internal string Description { get; private set; }
        internal Dictionary<int, ETLArgument> ArgumentsByOrder { get; private set; }

        internal ETLMode(string modeCommand, string modeName, string modeDescription)
            : base(modeCommand)
        {
            this.Name = modeName;
            this.Description = modeDescription;
            ArgumentsByOrder = new Dictionary<int, ETLArgument>();
            _counter = 1;
        }

        internal void AddArgument(ETLArgument argument)
        {
            argument.SetOrder(_counter);
            ArgumentsByOrder.Add(_counter, argument);
            _counter++;
        }

        internal bool ArgumentsAreValid()
        {
            var retVal = true;
            if (ArgumentsByOrder.Count() > 0 && ArgumentsByOrder.Values.Any(x => x.IsRequired == true) && ArgumentsByOrder.Values.Any(x => x.IsRequired == false))
            {
                var maxRequired = ArgumentsByOrder.Values.Where(x => x.IsRequired == true).Max(x => x.Order);
                var minOptional = ArgumentsByOrder.Values.Where(x => x.IsRequired == false).Min(x => x.Order);
                if (maxRequired > minOptional)
                {
                    retVal = false;
                }
            }
            return retVal;
        }
    }
}
