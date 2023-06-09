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
    //Free to Delete if after Dec 1st 2021
    public static class StoreLeasePaymentReport
    {
        /*
        private static readonly string sp_GetInfo = "etl.LeasePaymentsReportCreateRecord";

        public static void StoreReport(List<LeasePaymentReportModel> dataList)
        {
            StoreReportInDatabase(dataList);
        }

        //Can probably combine with StoreReport
        private static void StoreReportInDatabase(List<LeasePaymentReportModel> dataList)
        {
            foreach (var payment in dataList) { 
                try
                {
                    List<SqlParameter> parms = new List<SqlParameter>();
                    var sqlUtilCustAccountNumber = new SqlParameter("@UtilCustAccountNumber", SqlDbType.VarChar);
                    var sqlUtilLeaCustomerName = new SqlParameter("@UtilLeaCustomerName", SqlDbType.VarChar);
                    var sqlPROCDATE = new SqlParameter("@PROCDATE", SqlDbType.DateTime);
                    var sqlAmount = new SqlParameter("@Amount", SqlDbType.Decimal);
                    var sqlOffice = new SqlParameter("@Office", SqlDbType.Char);
                    var sqlName = new SqlParameter("@Name", SqlDbType.Char);
                    var sqlCashbox = new SqlParameter("@Cashbox", SqlDbType.Char);
                    var sqlCheckNum = new SqlParameter("@CheckNum", SqlDbType.Char);

                    sqlUtilCustAccountNumber.Value = payment.UtilCustAccountNumber;
                    sqlUtilLeaCustomerName.Value = payment.UtilLeaCustomerName;
                    sqlPROCDATE.Value = payment.ProcDate;
                    sqlAmount.Value = payment.Amount;
                    sqlOffice.Value = payment.Office;
                    sqlName.Value = payment.NAME;
                    sqlCashbox.Value = payment.Cashbox;
                    sqlCheckNum.Value = payment.CheckNum;

                    parms.Add(sqlUtilCustAccountNumber);
                    parms.Add(sqlUtilLeaCustomerName);
                    parms.Add(sqlPROCDATE);
                    parms.Add(sqlAmount);
                    parms.Add(sqlOffice);
                    parms.Add(sqlName);
                    parms.Add(sqlCashbox);
                    parms.Add(sqlCheckNum);

                    ExecuteDataset(Database.FNBCustom, sp_GetInfo, parms);

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        */
    }
}
