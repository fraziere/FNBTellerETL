using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.LeasePaymentReport
{
    public class LeasePaymentReportFormatedModel
    {
        public LeasePaymentReportFormatedModel()
        {
            UtilCustAccountNumber = "";
            UtilLeaCustomerName = "";
            ProcDate = "";
            Amount = "";
            ARGOAmount = "";
            Region = "";
            Office = "";
            Cashbox = "";
            TranSeq = "";
            MsgSeq = "";
            OperatorID = "";
            NAME = "";
            field6 = "";
            GUID = "";
            chgDateTime = "";
            source = "";
            CRDR = "";
            itemnumber = "";
            serial = "";
            abanumber = "";
            field4 = "";
            account = "";
            trancode = "";
            transeqSummeryNum = "";
        }

        public string UtilCustAccountNumber { get; set; }
        public string UtilLeaCustomerName { get; set; }
        public string ProcDate { get; set; }
        //public DateTime ProcDate { get; set; }
        public string Amount { get; set; }
        public string ARGOAmount { get; set; }
        public string Region { get; set; }
        public string Office { get; set; }
        public string Cashbox { get; set; }
        public string TranSeq { get; set; }
        public string MsgSeq { get; set; }
        public string OperatorID { get; set; }
        public string NAME { get; set; }
        public string field6 { get; set; }
        public string GUID { get; set; }
        //public int sequence { get; set; }
        public string ISN { get; set; }
        public string chgDateTime { get; set; }
        //public DateTime chgDateTime { get; set; }
        public string source { get; set; }
        public string CRDR { get; set; }
        public string itemnumber { get; set; }
        public string serial { get; set; }
        public string abanumber { get; set; }
        public string field4 { get; set; }
        public string account { get; set; }
        public string trancode { get; set; }
        public string transeqSummeryNum { get; set; }
    }
}
