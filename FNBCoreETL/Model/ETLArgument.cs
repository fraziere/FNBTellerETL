using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    internal class ETLArgument
    {
        internal int Order { get; private set; }

        internal string Name { get; private set; }

        internal string Description { get; private set; }

        internal bool IsRequired { get; private set; }

        internal Type ArgType { get; private set; }

        internal ETLArgument(string name, string argDescription, Type argType, bool isRequired = true)
        {
            this.Name = name;
            this.Description = argDescription;
            this.IsRequired = isRequired;
            this.ArgType = argType;
        }
        internal void SetOrder(int order)
        {
            Order = order;
        }
    }
}
