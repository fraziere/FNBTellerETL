using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Model
{
    internal class ETLModelException : System.Exception
    {
        internal ETLModelException() { }

        internal ETLModelException(string msg) : base(msg) { }

        internal ETLModelException(string msg, System.Exception innerEx) : base(msg, innerEx) { }
    }
}
