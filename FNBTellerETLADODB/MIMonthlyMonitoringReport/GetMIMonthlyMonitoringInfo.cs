using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.MIMonthlyMonitoringReport
{
    public static class GetMIMonthlyMonitoringInfo
    {


        private static readonly string sp_getMIMonthlyMonitoringReport = "etl.GetMIMonthlyMonitoring";

        /// <summary>
        /// Runs stored procedure against the view [FRAUD].[dbo].[vw_BSA_monetary_instruments_and_conductors]
        /// </summary>
        /// <returns></returns>
        public static List<MIMonthlyMonitoringModel> Get()
        {
            var retVal = new List<MIMonthlyMonitoringModel>();
            var ds = GetDataSet();

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new MIMonthlyMonitoringModel();
                    holder.transaction_date = dr.Field<DateTime>("transaction_date");
                    holder.transaction_code = dr.Field<string>("transaction_code");
                    holder.transaction_description = (dr.Field<string>("transaction_description") ?? "").Replace(',', ' ').Trim();
                    holder.serial_number = dr.Field<string>("serial_number");
                    holder.amount = dr.Field<decimal>("amount");
                    holder.funding_transaction_type = dr.Field<string>("funding_transaction_type");
                    holder.funding_account_type_code = dr.Field<string>("funding_account_type_code");
                    holder.funding_account_number = dr.Field<string>("funding_account_number");
                    holder.payee_name = (dr.Field<string>("payee_name") ?? "").Replace(',', ' ').Trim();
                    holder.urchaser_name = (dr.Field<string>("urchaser_name") ?? "").Replace(',', ' ').Trim();
                    holder.pur_customer_type = dr.Field<string>("pur_customer_type");
                    holder.pur_internal_type = dr.Field<string>("pur_internal_type");
                    holder.funding_transaction_number = dr.Field<string>("funding_transaction_number");
                    holder.transaction_number = dr.Field<string>("transaction_number");
                    holder.fee_amount = dr.Field<decimal>("fee_amount");
                    holder.fee_funded_by_cash = dr.Field<string>("fee_funded_by_cash");
                    holder.tin_type = dr.Field<string>("tin_type");
                    holder.tin_descr = (dr.Field<string>("tin_descr") ?? "").Replace(',', ' ').Trim();
                    holder.tin_number = dr.Field<string>("tin_number");
                    holder.recourse_acct_type_code = dr.Field<string>("recourse_acct_type_code");
                    holder.recourse_acct_number = dr.Field<string>("recourse_acct_number");
                    holder.recourse_transit = dr.Field<string>("recourse_transit");
                    holder.teller_id = (dr.Field<string>("teller_id") ?? "").Replace(',', ' ').Trim();
                    holder.teller_name = (dr.Field<string>("teller_name") ?? "").Replace(',', ' ').Trim();
                    holder.branch_code = dr.Field<string>("branch_code");

                    retVal.Add(holder);
                }
            }
            return retVal;
        }

        public static DataSet GetDataSet()
        {
            //Note: query takes around 40 seconds to run in PRD, added some extra seconds to be safe
            return ExecuteDataset(Database.FNBCustom, sp_getMIMonthlyMonitoringReport, timeOutInSeconds: 180);
        }
    }

    public class MIMonthlyMonitoringModel
    {
        public DateTime transaction_date { get; set; }
        public string transaction_code { get; set; }
        public string transaction_description { get; set; }
        public string serial_number { get; set; }
        public decimal amount { get; set; }
        public string funding_transaction_type { get; set; }
        public string funding_account_type_code { get; set; }
        public string funding_account_number { get; set; }
        public string payee_name { get; set; }
        public string urchaser_name { get; set; }
        public string pur_customer_type { get; set; }
        public string pur_internal_type { get; set; }
        public string funding_transaction_number { get; set; }
        public string transaction_number { get; set; }
        public decimal fee_amount { get; set; }
        public string fee_funded_by_cash { get; set; }
        public string tin_type { get; set; }
        public string tin_descr { get; set; }
        public string tin_number { get; set; }
        public string recourse_acct_type_code { get; set; }
        public string recourse_acct_number { get; set; }
        public string recourse_transit { get; set; }
        public string teller_id { get; set; }
        public string teller_name { get; set; }
        public string branch_code { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{transaction_date},{transaction_code},{transaction_description},");
            sb.Append($"{serial_number},{amount},{funding_transaction_type},");
            sb.Append($"{funding_account_type_code},{funding_account_number},{payee_name},{urchaser_name},{pur_customer_type},{pur_internal_type},{funding_transaction_number},{transaction_number},{fee_amount},");
            sb.Append($"{fee_funded_by_cash},{tin_type},{tin_descr},{tin_number},{recourse_acct_type_code},{recourse_acct_number},{recourse_transit},{teller_id},{teller_name},{branch_code}");

            return sb.ToString();
        }


        /*
         "Transaction Date, Transaction Code, Transaction Description, Serial Number, Amount, Funding Transaction Type," +
                           "Funding Account Type Code, Funding Account Number, Payee Name, Purchaser Name, Pur Customer Type, Pur Internal Type," +

                           "Funding Transaction Number, Transaction Number, Fee Amount, Fee Funded by Cash, Tin Type, Tin Descr, Tin Number, " +
                           "Recourse Acct Type Code, Recourse Acct Number, Recourse Transit, Teller Id, Teller Name, Branch Code");
         * 
         */

    }

}
