using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.TransitDepositReport
{
    public static class GetTellerTransactionData
    {
        private static readonly string sp_getTellerTransactionData = "etl.GetTellerTransactionData";

        public static List<TellerTranModel> Get(DateTime from, DateTime to)
        {
            var retVal = new List<TellerTranModel>();

            var ds = GetDataSet(from, to);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new TellerTranModel();
                    holder.procDate = dr.Field<DateTime>("procdate");
                    holder.regionID = dr.Field<string>("region_id");
                    holder.branchNumber = dr.Field<string>("office_id");
                    holder.officeName = dr.Field<string>("officeName");
                    holder.cashboxNumber = dr.Field<string>("cashsum_id");
                    holder.tranSequence = dr.Field<int>("transeq");
                    holder.GUID = dr.Field<string>("GUID");
                    holder.sequence = dr.Field<int>("sequence");
                    holder.ISN = dr.Field<string>("ISN");
                    holder.source = dr.Field<string>("source");
                    holder.operId = dr.Field<string>("oper_id");
                    holder.CRDR = dr.Field<string>("CRDR");
                    holder.itemnumber = dr.Field<int>("itemnumber");
                    holder.serial = dr.Field<string>("serial");
                    holder.field6 = dr.Field<string>("field6");
                    holder.abanumber = dr.Field<string>("abanumber");
                    holder.field4 = dr.Field<string>("field4");
                    holder.account = dr.Field<string>("account");
                    holder.trancode = dr.Field<string>("trancode");
                    holder.amount = dr.Field<decimal>("amount");

                    //(!row.IsNull(

                    //var temp0 = dr.IsNull("TransactionSumNum");
                    if (!dr.IsNull("TransactionSumNum"))
                    {
                        holder.transactionSumNum = dr.Field<int>("TransactionSumNum");
                        holder.itemCount = dr.Field<short>("ITEMCOUNT");
                        holder.creditAmount = dr.Field<double>("CREDITAMOUNT");
                        holder.creditCount = dr.Field<short>("CREDITCOUNT");
                        holder.debitAmount = dr.Field<double>("DEBITAMOUNT");
                        holder.debitCount = dr.Field<short>("DEBITCOUNT");
                    }
                    holder.tranID = dr.Field<string>("TRANID");
                    holder.procListName = dr.Field<string>("PROCLISTNAME");
                    holder.acctNum = dr.Field<string>("ACCTNUM");
                    holder.acctType1 = dr.Field<string>("ACCTYPE1");
                    holder.amount1 = dr.Field<double>("AMOUNT1");

                    retVal.Add(holder);
                }
            }

            return retVal;
        }


        private static DataSet GetDataSet(DateTime from, DateTime to)
        {
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlFrom = new SqlParameter("@ProcDateLow", SqlDbType.DateTime);
                sqlFrom.Value = from;
                parms.Add(sqlFrom);
                var sqlTo = new SqlParameter("@ProcDateHigh", SqlDbType.DateTime);
                sqlTo.Value = to;
                parms.Add(sqlTo);
                return ExecuteDataset(Database.FNBCustom, sp_getTellerTransactionData, parms, timeOutInSeconds: 900);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public struct TellerTranModel
    {
        public DateTime procDate { get; set; }
        public string regionID { get; set; }
        public string branchNumber { get; set; }
        public string officeName { get; set; }
        public string cashboxNumber { get; set; }
        public int tranSequence { get; set; }
        public string GUID { get; set; }
        public int sequence { get; set; }
        public string ISN { get; set; }
        public string source { get; set; }
        public string operId { get; set; }
        public string CRDR { get; set; }
        public int itemnumber { get; set; }
        public string serial { get; set; }
        public string field6 { get; set; }
        public string abanumber { get; set; }
        public string field4 { get; set; }
        public string account { get; set; }
        public string trancode { get; set; }
        public decimal amount { get; set; }

        public int transactionSumNum { get; set; }
        public short itemCount { get; set; }
        public double creditAmount { get; set; }
        public short creditCount { get; set; }
        public double debitAmount { get; set; }
        public short debitCount { get; set; }
        public string tranID { get; set; }
        public string procListName { get; set; }
        public string acctNum { get; set; }
        public string acctType1 { get; set; }
        public double amount1 { get; set; }
    }
}
