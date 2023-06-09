using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Framework
{
    public class ETLJobOut
    {
        public int RecordCount { get; private set; }
        public string OutputToLog { get; private set; }

        public ETLJobOut(int recordCount, string outputToLog)
        {
            this.RecordCount = recordCount;
            this.OutputToLog = outputToLog;
        }
    }
    
}
