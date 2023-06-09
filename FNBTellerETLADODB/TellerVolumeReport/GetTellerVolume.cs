using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.TellerVolumeReport
{
    public static class GetTellerVolume
    {
        private static readonly string sp_GetInfo = "etl.TellerVolumeReport_DateRange";

        /// <summary>
        /// Executes stored procedure to get teller transactional data between from and to dates.
        /// Returns a list of TellerVolumeModel objects each holding the transactional data for a teller.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<TellerVolumeModel> Get(DateTime from, DateTime to)
        {
            var retVal = new List<TellerVolumeModel>();

            var ds = GetDataSet(from, to);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new TellerVolumeModel();
                    holder.regionName = dr.Field<string>("Region");
                    holder.regionID = dr.Field<string>("Region ID"); 
                    holder.branchName = dr.Field<string>("Branch Name");
                    holder.branchNumber = dr.Field<string>("Branch Number");
                    holder.cashboxNumber = dr.Field<string>("Cashbox Number");
                    holder.tellerName = dr.Field<string>("Teller Name");
                    holder.lobbyTranscations = dr.Field<int>("Lobby Transactions");
                    holder.driveUpTransactions = dr.Field<int>("Drive Up Transactions");
                    holder.unspecifiedTransactions = dr.Field<int>("UnSpecified Transactions");
                    holder.totalThisMonthTransactions = dr.Field<int>("Total Transaction Count");
                    holder.totalLastMonthTransactions = -1;

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
                return ExecuteDataset(Database.FNBCustom, sp_GetInfo, parms, 900);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
