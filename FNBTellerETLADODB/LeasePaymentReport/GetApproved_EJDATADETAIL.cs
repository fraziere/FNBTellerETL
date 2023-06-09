using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.LeasePaymentReport
{
    public static class GetApproved_EJDATADETAIL
    {
        private static readonly string sp_GetEJInfo = "etl.CheckAgainstEJDATADETAIL";
        public static List<EJDataInfo> RunEJDataDetailApprovedQuery(string region_id, string office_id, string transeq, string msgseq, string cashsum_id, DateTime procdate)
        {
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlRegionId = new SqlParameter("@RegionID", SqlDbType.VarChar);
                sqlRegionId.Value = region_id;
                parms.Add(sqlRegionId);
                var sqlOfficeID = new SqlParameter("@OfficeID", SqlDbType.VarChar);
                sqlOfficeID.Value = office_id;
                parms.Add(sqlOfficeID);
                var sqlTransSeq = new SqlParameter("@TransSeq", SqlDbType.VarChar);
                sqlTransSeq.Value = transeq;
                parms.Add(sqlTransSeq);
                var sqlMsgSeq = new SqlParameter("@MsgSeq", SqlDbType.VarChar);
                sqlMsgSeq.Value = msgseq;
                parms.Add(sqlMsgSeq);
                var sqlCashsumId = new SqlParameter("@CashsumId", SqlDbType.VarChar);
                sqlCashsumId.Value = cashsum_id;
                parms.Add(sqlCashsumId);
                var sqlProcDate = new SqlParameter("@ProcDate", SqlDbType.DateTime);
                sqlProcDate.Value = procdate;
                parms.Add(sqlProcDate);

                var outputList = new List<EJDataInfo>();



                foreach (DataRow dr in ExecuteDataset(Database.FNBCustom, sp_GetEJInfo, parms).Tables[0].Rows)
                {
                    var holder = new EJDataInfo();
                    holder.regionID = dr.Field<string>("REGIONID");
                    holder.officeID = dr.Field<string>("OFFICEID");
                    holder.CashSumID = dr.Field<string>("CASHSUMID");
                    holder.TransSeqNum = dr.Field<int>("TRANSEQNUM");
                    holder.MsgSeqNum = dr.Field<short>("MSGSEQNUM");
                    holder.reversal = dr.Field<short>("REVERSAL");
                    holder.reversed = dr.Field<short>("REVERSED");

                    outputList.Add(holder);
                }


                return outputList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }


    public struct EJDataInfo        
    {
        public string regionID { get; set; }
        public string officeID { get; set; }
        public string CashSumID { get; set; }
        public int TransSeqNum { get; set; }
        public short MsgSeqNum { get; set; }
        public short reversal { get; set; }
        public short reversed { get; set; }

        //May add more in future
    }
}
