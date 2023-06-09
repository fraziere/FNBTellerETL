using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.LeasePaymentReport
{
    public class LeasePaymentReportARGOBlobModel
    {
        public string UtilCustAccountNumber { get; set; }
        public string UtilLeaCustomerName { get; set; }
        public DateTime ProcDate { get; set; }
        public string Amount { get; set; }
        public string ARGOAmount { get; set; }
        public string Region { get; set; }          //new
        public string Office { get; set; }
        public string Cashbox { get; set; }
        public string TranSeq { get; set; }         //new
        public string MsgSeq { get; set; }          //new
        public string OperatorID { get; set; }      //new
        public string NAME { get; set; }        
        public string TranCode { get; set; }
        public string field6 { get; set; }
        public string SerialNum { get; set; }
    }
}
