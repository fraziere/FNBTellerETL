using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.CashAdvanceReport
{
    public static class GetCashAdvanceReport
    {
        private static readonly string sp_GetInfo = "etl.CashAdvanceReport";

        public static List<CashAdvanceReportModel> Get(DateTime from, DateTime to)
        {
            var retVal = new List<CashAdvanceReportModel>();

            var ds = GetDataSet(from, to);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new CashAdvanceReportModel();
                    holder.RegionId = dr.Field<string>("RegionId");
                    holder.OfficeId = dr.Field<string>("OfficeId");
                    holder.OfficeName = dr.Field<string>("OfficeName");
                    holder.ProcessingDate = dr.Field<DateTime?>("ProcessingDate");
                    holder.TotalAmount = dr.Field<decimal?>("TotalAmount");

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
                var sqlFrom = new SqlParameter("@From", SqlDbType.DateTime);
                sqlFrom.Value = from;
                parms.Add(sqlFrom);
                var sqlTo = new SqlParameter("@To", SqlDbType.DateTime);
                sqlTo.Value = to;
                parms.Add(sqlTo);
                return ExecuteDataset(Database.FNBCustom, sp_GetInfo, parms);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
