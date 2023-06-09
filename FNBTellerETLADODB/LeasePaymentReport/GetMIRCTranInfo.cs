using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.LeasePaymentReport
{
    public static class GetMIRCTranInfo
    {
        private static readonly string sp_GetInfo = "etl.CheckMICRInfo";
        private static readonly string sp_GetMinTranSum = "etl.LPRTranSumPrevious";
        private static readonly string sp_GetMaxTranSum = "etl.LPRTranSumCurrent";
        private static readonly string sp_GetAllTransInScession = "etl.LPRGetSessionAllTrans";


        /// <summary>
        /// Executes stored procedure against [ARGOENT].[dbo].[TLR_ITEM_MICR] table.
        /// Returns a list of MICR details for the transactions that match the specified parameters.
        /// </summary>
        public static List<MICRTranSPInfo> RunMICRTranQuery(string region_id, string office_id, string transeq, string cashsum_id, DateTime procdate)
        {
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlRegionId = new SqlParameter("@RegionID", SqlDbType.Char);
                sqlRegionId.Value = region_id;
                parms.Add(sqlRegionId);
                var sqlOfficeID = new SqlParameter("@OfficeID", SqlDbType.Char);
                sqlOfficeID.Value = office_id;
                parms.Add(sqlOfficeID);
                var sqlTransSeq = new SqlParameter("@TransSeq", SqlDbType.Int);
                sqlTransSeq.Value = transeq;
                parms.Add(sqlTransSeq);
                var sqlCashsumId = new SqlParameter("@CashsumId", SqlDbType.Char);
                sqlCashsumId.Value = cashsum_id;
                parms.Add(sqlCashsumId);
                var sqlProcDate = new SqlParameter("@ProcDate", SqlDbType.DateTime);
                sqlProcDate.Value = procdate;
                parms.Add(sqlProcDate);
                
                var outputList = new List<MICRTranSPInfo>();
                foreach (DataRow dr in ExecuteDataset(Database.FNBCustom, sp_GetInfo, parms).Tables[0].Rows)
                {
                    var holder = new MICRTranSPInfo();
                    holder.Sequence = dr.Field<int>("sequence");
                    holder.ISN = dr.Field<string>("ISN");
                    holder.Serial = dr.Field<string>("serial");
                    holder.field6 = dr.Field<string>("field6");
                    holder.TranCode = dr.Field<string>("trancode").Trim(' ');
                    holder.Amount = dr.Field<decimal>("amount");

                    outputList.Add(holder);
                }
                outputList = outputList
                        .GroupBy(a => a.ISN)
                        .Select(g => g.OrderByDescending(a => a.Sequence).First())
                        .ToList();

                return outputList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns all relevant transactions given the parameters
        /// </summary>
        /// <returns></returns>
        public static List<LeasePaymentTransactionsOfIntrestModel> GetTransactionInfo(string region_id, string office_id, string transeq, string cashsum_id, DateTime procdate)
        {
            int previousTranNumber = GetMinTranSeqNum(region_id, office_id, transeq, cashsum_id, procdate);

            var sumVar = GetTranSeqNumInfo(region_id, office_id, transeq, cashsum_id, procdate);

            return GetAllSessionTransactionInfo(region_id, office_id, previousTranNumber.ToString(), sumVar.Sequence.ToString(), cashsum_id, procdate);
        }

        /// <summary>
        /// Calls stored procedure which runs against [ARGOENT].[dbo].[TLR_EJTRAN] table. Returns the minimum (starting) tran sequence number given the parameters.
        /// </summary>
        /// <returns></returns>
        private static int GetMinTranSeqNum(string region_id, string office_id, string transeq, string cashsum_id, DateTime procdate)
        {
            var outputValue = 0;

            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlRegionId = new SqlParameter("@RegionID", SqlDbType.Char);
                sqlRegionId.Value = region_id;
                parms.Add(sqlRegionId);
                var sqlOfficeID = new SqlParameter("@OfficeID", SqlDbType.Char);
                sqlOfficeID.Value = office_id;
                parms.Add(sqlOfficeID);
                var sqlTransSeq = new SqlParameter("@TransSeq", SqlDbType.Int);
                sqlTransSeq.Value = transeq;
                parms.Add(sqlTransSeq);
                var sqlCashsumId = new SqlParameter("@CashsumId", SqlDbType.Char);
                sqlCashsumId.Value = cashsum_id;
                parms.Add(sqlCashsumId);
                var sqlProcDate = new SqlParameter("@ProcDate", SqlDbType.DateTime);
                sqlProcDate.Value = procdate;
                parms.Add(sqlProcDate);

                //should only run once
                foreach (DataRow dr in ExecuteDataset(Database.FNBCustom, sp_GetMinTranSum, parms).Tables[0].Rows)
                {
                    outputValue = dr.Field<int>("TRANSEQNUM");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return outputValue;
        }

        /// <summary>
        /// Calls stored procedure which runs against [ARGOENT].[dbo].[TLR_EJTRAN] table. Returns the maximum (ending) tran sequence number given the parameters.
        /// </summary>
        /// <returns></returns>
        private static EJTranSumInfo GetTranSeqNumInfo(string region_id, string office_id, string transeq, string cashsum_id, DateTime procdate)
        {
            EJTranSumInfo returnVal = new EJTranSumInfo();

            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlRegionId = new SqlParameter("@RegionID", SqlDbType.Char);
                sqlRegionId.Value = region_id;
                parms.Add(sqlRegionId);
                var sqlOfficeID = new SqlParameter("@OfficeID", SqlDbType.Char);
                sqlOfficeID.Value = office_id;
                parms.Add(sqlOfficeID);
                var sqlTransSeq = new SqlParameter("@TransSeq", SqlDbType.Int);
                sqlTransSeq.Value = transeq;
                parms.Add(sqlTransSeq);
                var sqlCashsumId = new SqlParameter("@CashsumId", SqlDbType.Char);
                sqlCashsumId.Value = cashsum_id;
                parms.Add(sqlCashsumId);
                var sqlProcDate = new SqlParameter("@ProcDate", SqlDbType.DateTime);
                sqlProcDate.Value = procdate;
                parms.Add(sqlProcDate);

                //should only run once
                foreach (DataRow dr in ExecuteDataset(Database.FNBCustom, sp_GetMaxTranSum, parms).Tables[0].Rows)
                {
                    returnVal.Sequence = dr.Field<int>("TRANSEQNUM");
                    returnVal.ItemCount = dr.Field<short>("ITEMCOUNT");
                    returnVal.CreditAmount = dr.Field<double>("CREDITAMOUNT");
                    returnVal.CreditCount = dr.Field<short>("CREDITCOUNT");
                    returnVal.DebitAmount = dr.Field<double>("DEBITAMOUNT");
                    returnVal.DebitCount = dr.Field<short>("DEBITCOUNT");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return returnVal;
        }

        /// <summary>
        /// Calls stored procedure which runs against [ARGOENT].[dbo].[TLR_ITEM_MICR] table.
        /// Returns a List of Transactions that occurred between the transeqLow and transeqHigh values.
        /// These transactions are the ones that occurred during a session.
        /// </summary>
        /// <returns></returns>
        private static List<LeasePaymentTransactionsOfIntrestModel> GetAllSessionTransactionInfo(string region_id, string office_id, string transeqLow, string transeqHigh, string cashsum_id, DateTime procdate)
        {
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlRegionId = new SqlParameter("@RegionID", SqlDbType.Char);
                sqlRegionId.Value = region_id;
                parms.Add(sqlRegionId);
                var sqlOfficeID = new SqlParameter("@OfficeID", SqlDbType.Char);
                sqlOfficeID.Value = office_id;
                parms.Add(sqlOfficeID);
                var sqlTransSeqHigh = new SqlParameter("@TransSeqHigh", SqlDbType.Int);
                sqlTransSeqHigh.Value = transeqHigh;
                parms.Add(sqlTransSeqHigh);
                var sqlTransSeqLow = new SqlParameter("@TransSeqLow", SqlDbType.Int);
                sqlTransSeqLow.Value = transeqLow;
                parms.Add(sqlTransSeqLow);
                var sqlCashsumId = new SqlParameter("@CashsumId", SqlDbType.Char);
                sqlCashsumId.Value = cashsum_id;
                parms.Add(sqlCashsumId);
                var sqlProcDate = new SqlParameter("@ProcDate", SqlDbType.DateTime);
                sqlProcDate.Value = procdate;
                parms.Add(sqlProcDate);


                var outputList = new List<LeasePaymentTransactionsOfIntrestModel>();
                foreach (DataRow dr in ExecuteDataset(Database.FNBCustom, sp_GetAllTransInScession, parms).Tables[0].Rows)
                {
                    var holder = new LeasePaymentTransactionsOfIntrestModel();
                    holder.procdate = dr.Field<DateTime>("procdate");
                    holder.region_id = dr.Field<string>("region_id");
                    holder.office_id = dr.Field<string>("office_id");
                    holder.cashsum_id = dr.Field<string>("cashsum_id");
                    holder.transeq = dr.Field<int>("transeq");
                    holder.GUID = dr.Field<string>("GUID");
                    holder.sequence = dr.Field<int>("sequence");
                    holder.ISN = dr.Field<string>("ISN");
                    holder.chgDateTime = dr.Field<DateTime>("chgDateTime");
                    holder.source = dr.Field<string>("source");
                    holder.oper_id = dr.Field<string>("oper_id");
                    holder.CRDR = dr.Field<string>("CRDR");
                    holder.itemnumber = dr.Field<int>("itemnumber");
                    holder.serial = dr.Field<string>("serial");
                    holder.field6 = dr.Field<string>("field6");
                    holder.abanumber = dr.Field<string>("abanumber");
                    holder.field4 = dr.Field<string>("field4");
                    holder.account = dr.Field<string>("account");
                    holder.trancode = dr.Field<string>("trancode");
                    holder.amount = dr.Field<decimal>("amount");
                    holder.REVERSAL = dr.Field<short>("REVERSAL");
                    holder.REVERSED = dr.Field<short>("REVERSED");
                    holder.transeqSummeryNumber = transeqHigh;


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

    public struct EJTranMICRInfo
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
    }

    public struct EJTranSumInfo
    {
        public int Sequence { get; set; }
        public short ItemCount { get; set; }
        public double CreditAmount { get; set; }
        public short CreditCount { get; set; }
        public double DebitAmount { get; set; }
        public short DebitCount { get; set; }
    }

    public struct MICRTranSPInfo
    {
        public int Sequence { get; set; }
        public string ISN { get; set; }
        public string Serial { get; set; }
        public string field6 { get; set; }
        public string TranCode { get; set; }
        public decimal Amount { get; set; }
    }
}
