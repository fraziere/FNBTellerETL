using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.LeasePaymentReport
{
    public class LeasePaymentTransactionsOfIntrestModel
    {
        public DateTime procdate { get; set; }
        public string region_id { get; set; }
        public string office_id { get; set; }
        public string cashsum_id { get; set; }
        public int transeq { get; set; }
        public string GUID { get; set; }
        public int sequence { get; set; }
        public string ISN { get; set; }
        public DateTime chgDateTime { get; set; }
        public string source { get; set; }
        public string oper_id { get; set; }
        public string CRDR { get; set; }
        public int itemnumber { get; set; }
        public string serial { get; set; }
        public string field6 { get; set; }
        public string abanumber { get; set; }
        public string field4 { get; set; }
        public string account { get; set; }
        public string trancode { get; set; }
        public decimal amount { get; set; }
        public short REVERSAL { get; set; }
        public short REVERSED { get; set; }
        public string transeqSummeryNumber { get; set; }
    }
}
