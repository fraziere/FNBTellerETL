using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL.Config
{
    internal class FileConfigurationException : System.Exception
    {
        internal FileConfigurationException() { }

        internal FileConfigurationException(string msg) : base(msg) { }

        internal FileConfigurationException(string msg, System.Exception innerEx) : base(msg, innerEx) { }
    }
}
