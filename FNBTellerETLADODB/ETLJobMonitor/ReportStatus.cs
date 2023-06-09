using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.ETLJobMonitor
{
    public struct ReportStatus
    {
        public int reportId { get; set; }
        public string command { get; set; }
        public string commandName { get; set; }
        public string reportEnvironment { get; set; }
        public string reportFrequency { get; set; }
        public DateTime createdDate { get; set; }
    }
}
