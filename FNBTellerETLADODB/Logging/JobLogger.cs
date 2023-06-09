using FNBCoreETL.Logging;
using FNBCoreETL.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB.Logging
{
    internal static class JobLogger
    {
        private static readonly string sp_ApplicationLogStore = "etl.JobLogInsert";

        internal static void Log(ETLJobLogEventArgs e)
        {
            List<SqlParameter> sParams = new List<SqlParameter>();

            SqlParameter sqlIsError = new SqlParameter("@IsError", SqlDbType.Bit);
            sqlIsError.Value = e.IsError;
            sParams.Add(sqlIsError);
            sParams.Add(new SqlParameter("@Severity", e.Severity.ToString()));
            sParams.Add(new SqlParameter("@Environment", Bootstrap.Environment));
            sParams.Add(new SqlParameter("@Command", e.Command));
            sParams.Add(new SqlParameter("@CommandName", e.CommandName));
            sParams.Add(new SqlParameter("@Mode", e.Mode));
            sParams.Add(new SqlParameter("@ModeName", e.ModeName));
            sParams.Add(new SqlParameter("@Arguments", e.Arguments));
            sParams.Add(new SqlParameter("@ShortMsg", e.ShortMsg));
            sParams.Add(new SqlParameter("@LongMsg", e.LongMsg));
            SqlParameter sqlRecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);
            sqlRecordCount.Value = e.RecordCount;
            sParams.Add(sqlRecordCount);
            SqlParameter sqlRoundTrip = new SqlParameter("@RoundTripMs", SqlDbType.Float);
            sqlRoundTrip.Value = e.RoundTripMs;
            sParams.Add(sqlRoundTrip);
            sParams.Add(new SqlParameter("@Output", e.Output));
            sParams.Add(new SqlParameter("@ServerName", Bootstrap.MachineName));
            sParams.Add(new SqlParameter("@SessionID", Bootstrap.SessionId));

            ADODB.ExecuteNonQuery(ADODB.Database.FNBCustom, sp_ApplicationLogStore, sParams);

            
        }
    }
}
