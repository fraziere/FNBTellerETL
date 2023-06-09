using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.TellerVolumeReport
{
    public class BranchVolumeModel
    {
        public string regionID { get; set; }
        public string branchName { get; set; }
        public string branchNumber { get; set; }
        public int totalThisMonthTransactions { get; set; }
        public int totalLastMonthTransactions { get; set; }
    }
}
