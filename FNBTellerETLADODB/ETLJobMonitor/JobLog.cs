using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.ETLJobMonitor
{
    public struct JobLog
    {
        public int jobLogId { get; set; }
        public bool isError { get; set; }
        public string severity { get; set; }
        public string environment { get; set; }
        public DateTime createDate { get; set; }
        public string command { get; set; }
        public string commandName { get; set; }
        public string mode { get; set; }
        public string modeName { get; set; }
        public string arguments;
        public string shortMsg;
        public string longMsg;
        public int recordCount { get; set; }
        public double? roundTripMs { get; set; }
        public string output;
        public string serverName;
        public Guid sessionID;

        public override string ToString()
        {
            return $"[{jobLogId},{isError},{severity},{environment},{createDate},{command},{commandName},{recordCount}]";
        }
    }
}
