using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Framework
{
    internal class ETLFrameworkExcpetion : System.Exception
    {
        internal ETLFrameworkExcpetion() { }

        internal ETLFrameworkExcpetion(string msg) : base(msg) { }

        internal ETLFrameworkExcpetion(string msg, System.Exception innerEx) : base(msg, innerEx) { }
    }
}
