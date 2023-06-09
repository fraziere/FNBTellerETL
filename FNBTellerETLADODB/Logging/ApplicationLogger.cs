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
    internal static class ApplicationLogger
    {
        private static readonly string sp_ApplicationLogStore = "etl.ApplicationLogInsert";

        internal static void Log(ETLAppLogEventArgs e)
        {

            try
            {
                List<SqlParameter> sParams = new List<SqlParameter>();

                sParams.Add(new SqlParameter("@State", e.State.ToString()));
                SqlParameter sqlIsError = new SqlParameter("@IsError", SqlDbType.Bit);
                sqlIsError.Value = e.IsError;
                sParams.Add(sqlIsError);
                sParams.Add(new SqlParameter("@Severity", e.Severity.ToString()));
                sParams.Add(new SqlParameter("@Environment", Bootstrap.Environment));
                sParams.Add(new SqlParameter("@Command", e.Command));
                sParams.Add(new SqlParameter("@CommandName", e.CommandName));
                SqlParameter sqlIsChainedCommand = new SqlParameter("@IsChainedCommand", SqlDbType.Bit);
                sqlIsChainedCommand.Value = e.IsChainedCommand;
                sParams.Add(sqlIsChainedCommand);
                sParams.Add(new SqlParameter("@Mode", e.Mode));
                sParams.Add(new SqlParameter("@ModeName", e.ModeName));
                sParams.Add(new SqlParameter("@Arguments", e.Arguments));
                sParams.Add(new SqlParameter("@ShortMsg", e.ShortMsg));
                sParams.Add(new SqlParameter("@LongMsg", e.LongMsg));
                SqlParameter sqlRecordCount = new SqlParameter("@RecordCount", SqlDbType.Int);
                sqlRecordCount.Value = e.RecordCount;
                sParams.Add(sqlRecordCount);
                SqlParameter sqlRoundTrip = new SqlParameter("@RoundTripSec", SqlDbType.Float);
                sqlRoundTrip.Value = e.RoundTripSec;
                sParams.Add(sqlRoundTrip);
                sParams.Add(new SqlParameter("@ServerName", Bootstrap.MachineName));
                sParams.Add(new SqlParameter("@SessionID", Bootstrap.SessionId));

                //SqlParameter sqlOutput = new SqlParameter("@AppLogId", SqlDbType.BigInt);
                //sqlOutput.Direction = ParameterDirection.Output;
                //sParams.Add(sqlOutput);

                ADODB.ExecuteNonQuery(ADODB.Database.FNBCustom, sp_ApplicationLogStore, sParams);
            }
            catch (Exception ex)
            {
                try
                {
                    ServerLogUtil.TryWriteToServerLog("Error attempting to write to application log on database. Following messages will contain more info.", System.Diagnostics.EventLogEntryType.Error);
                    ServerLogUtil.TryWriteToServerLog(String.Format("Write to DB Exception: {0}", ex.ToString()), System.Diagnostics.EventLogEntryType.Error);
                    ServerLogUtil.TryWriteToServerLog(String.Format("Original Log Info: Severity = {0}, ShortMsg = {1}, LongMsg = {2}",
                        e.Severity.ToString(), e.ShortMsg, e.LongMsg), System.Diagnostics.EventLogEntryType.Error);
                }
                catch (Exception inEx)
                {
                    //if a console is available, nowhere else to go.
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(inEx.ToString());
                    throw;
                }
                throw;          
            }

        }
    }
}
