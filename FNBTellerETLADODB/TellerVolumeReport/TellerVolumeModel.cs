using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.TellerVolumeReport
{
    public class TellerVolumeModel
    {
        public string regionName { get; set; }
        public string regionID { get; set; }
        public string branchName { get; set; }
        public string branchNumber { get; set; }
        public string cashboxNumber { get; set; }
        public string tellerName { get; set; }
        public int lobbyTranscations { get; set; }
        public int driveUpTransactions { get; set; }
        public int unspecifiedTransactions { get; set; }
        public int totalThisMonthTransactions { get; set; }
        public int totalLastMonthTransactions { get; set; }
    }
}
