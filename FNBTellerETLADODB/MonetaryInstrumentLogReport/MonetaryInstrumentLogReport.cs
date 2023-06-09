using System;
using System.Collections.Generic;
using System.Data;
using static FNBTellerETLADODB.ADODB;


namespace FNBTellerETLADODB.MonetaryInstrumentLogReport
{
    public static class MonetaryInstrumentLogReport
    {
        private static readonly string sp_getLogReport = "etl.GetMonetaryInstrumentLog";

        /// <summary>
        /// Runs stored procedure against the view [FRAUD].[dbo].[vw_BSA_monetary_instruments_and_conductors]
        /// </summary>
        /// <returns></returns>
        public static List<MonetaryInstrumentLogModel> Get()
        {
            var retVal = new List<MonetaryInstrumentLogModel>();
            var ds = GetDataSet();

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new MonetaryInstrumentLogModel();
                    holder.FundingTransactionNumber = dr.Field<string>("funding_transaction_number");
                    holder.TransactionNumber = dr.Field<string>("transaction_number");
                    holder.TransactionDate = dr.Field<DateTime>("transaction_date");
                    holder.TransactionTime = dr.Field<DateTime>("transaction_time");
                    holder.CrDrInd = dr.Field<string>("cr_dr_ind");
                    holder.Amount = dr.Field<decimal?>("amount");
                    holder.FeeAmount = dr.Field<decimal?>("fee_amount");
                    holder.FeeFundedByCash = dr.Field<string>("fee_funded_by_cash");
                    holder.TransactionCode = dr.Field<string>("transaction_code");
                    holder.TransactionDescription = dr.Field<string>("transaction_description");
                    holder.Channel = dr.Field<string>("channel");
                    holder.TellerId = dr.Field<string>("teller_id");
                    holder.OfficeId = dr.Field<string>("office_id");
                    holder.BranchCode = dr.Field<string>("branch_code");
                    holder.BranchState = dr.Field<string>("branch_state");
                    holder.BranchZipCode = dr.Field<string>("branch_zip_code");
                    holder.SerialNumber = dr.Field<string>("serial_number");
                    holder.FundingTransactionType = dr.Field<string>("funding_transaction_type");
                    holder.FundingAccountTypeCode = dr.Field<string>("funding_account_type_code");
                    holder.FundingAccountNumber = dr.Field<string>("funding_account_number");
                    holder.FundingCheckNumber = dr.Field<string>("funding_check_number");
                    holder.PayeeName = dr.Field<string>("payee_name");
                    holder.Purchaser = dr.Field<string>("purchaser");
                    holder.PurCustomerType = dr.Field<string>("pur_customer_type");
                    holder.PurInternalType = dr.Field<string>("pur_internal_type");
                    holder.PurOccupation = dr.Field<string>("pur_occupation");
                    holder.PurAddress = dr.Field<string>("pur_address");
                    holder.PurCity = dr.Field<string>("pur_city");
                    holder.PurState = dr.Field<string>("pur_state");
                    holder.PurZipCode = dr.Field<string>("pur_zip_code");
                    holder.PurCountry = dr.Field<string>("pur_country");
                    holder.DateOfBirth = dr.Field<string>("date_of_birth");
                    holder.IdType = dr.Field<string>("id_type");
                    holder.IdDescr = dr.Field<string>("id_descr");
                    holder.IdNumber = dr.Field<string>("id_number");
                    holder.IdState = dr.Field<string>("id_state");
                    holder.IdIssuer = dr.Field<string>("id_issuer");
                    holder.TinType = dr.Field<string>("tin_type");
                    holder.TinDescr = dr.Field<string>("tin_descr");
                    holder.TinNumber = dr.Field<string>("tin_number");
                    holder.RecourseAcctNumber = dr.Field<string>("recourse_acct_number");
                    holder.RecourseTransit = dr.Field<string>("recourse_transit");
                    holder.RecourseAcctTypeCode = dr.Field<string>("recourse_acct_type_code");
                    holder.RecourseHostAppCode = dr.Field<string>("recourse_host_app_code");
                    holder.RecourseHostRegionCode = dr.Field<string>("recourse_host_region_code");

                    retVal.Add(holder);
                }
            }
            return retVal;
        }

        public static DataSet GetDataSet()
        {
            //Note: query takes around 40 seconds to run in PRD, added some extra seconds to be safe
            return ExecuteDataset(Database.FNBCustom, sp_getLogReport, timeOutInSeconds: 600);
        }
    }
}
