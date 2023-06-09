using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.CashAdvanceReport
{
    public class CashAdvanceReportModel
    {
        /// <summary>
        /// PK for Office in Argo Teller is combo of region id and office id in SYS_OFFICE
        /// </summary>
        public string RegionId { get; set; }
        /// <summary>
        /// PK for Office in Argo Teller is combo of region id and office id in SYS_OFFICE
        /// </summary>
        public string OfficeId { get; set; }
        public string OfficeName { get; set; }
        public DateTime? ProcessingDate { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
