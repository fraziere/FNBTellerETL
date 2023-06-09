using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Logging
{
    public static class Logger
    {
        public static class AppLog
        {
            public enum State { Start, End }
            public static void LogError(State state, Exception ex, ETLJobIn job)
            {
                Log(state, SeverityEnum.Error, job, null, null, ex?.Message ?? "", ex?.ToString() ?? "");
            }
            public static void LogError(State state, string shortMsg, string longMsg, ETLJobIn job)
            {
                Log(state, SeverityEnum.Error, job, null, null, shortMsg, longMsg);
            }
            public static void LogWarning(State state, string shortMsg, string longMsg, ETLJobIn job)
            {
                Log(state, SeverityEnum.Warning,job, null, null, shortMsg, longMsg);
            }
            public static void LogInfo(State state, string shortMsg, string longMsg, ETLJobIn job)
            {
                Log(state, SeverityEnum.Info, job, null, null, shortMsg, longMsg);
            }
            public static void LogInfo(State state, int? recordCount, double? roundTripsSec, string shortMsg, string longMsg, ETLJobIn job)
            {
                Log(state, SeverityEnum.Info, job, recordCount, roundTripsSec, shortMsg, longMsg);
            }
            private static void Log(
                    State state,
                    SeverityEnum severity,    
                    ETLJobIn etlJob,
                    int? recordCount, double? roundTripSec,
                    string shortMsg = "",
                    string longMsg = "")
            {

                var flattendArgs = new StringBuilder();
                if (etlJob?.ArgumentsByName != null)
                {
                    foreach (var kvp in etlJob.ArgumentsByName)
                    {
                        flattendArgs.Append($"{kvp.Key}:{kvp.Value},");
                    }
                }

                var eventArgs = new ETLAppLogEventArgs(state, severity, Bootstrap.Environment,
                    etlJob?.Command ?? "", etlJob?.CommandName ?? "", etlJob?.IsChained, 
                    etlJob?.ModeCommand ?? "", etlJob?.ModeName ?? "", flattendArgs.ToString().TrimEnd(','),
                    shortMsg, longMsg, recordCount, roundTripSec, Bootstrap.MachineName, Bootstrap.SessionId); ;

                EventManager.LogETLApplicationEvent(eventArgs);
            }
        }

        public static class JobLog
        {
            public static void LogInfo(int? recordCount, double? roundTripMs, ETLJobIn etlJob, string outputToLog)
            {
                Log(false, SeverityEnum.Info, etlJob, recordCount, roundTripMs, outputToLog, "", "");
            }
            public static void LogError(ETLJobIn etlJob, Exception ex)
            {
                Log(true, SeverityEnum.Error, etlJob, null, null,null, ex?.Message ?? "", ex?.ToString() ?? "");
            }

            private static void Log(bool isError, SeverityEnum severity, ETLJobIn etlJob,int? recordCount, double? roundTripMs, string output, string shortMsg, string longMsg)
            {

                var flattendArgs = new StringBuilder();
                if (etlJob?.ArgumentsByName != null)
                {
                    foreach (var kvp in etlJob.ArgumentsByName)
                    {
                        flattendArgs.Append($"{kvp.Key}:{kvp.Value},");
                    }
                }

                var eventArgs = new ETLJobLogEventArgs(isError, severity, Bootstrap.Environment,
                         etlJob?.Command ?? "", etlJob?.CommandName ?? "", 
                         etlJob?.ModeCommand ?? "", etlJob?.ModeName ?? "", flattendArgs.ToString().TrimEnd(','),
                         shortMsg, longMsg, recordCount, roundTripMs, output, Bootstrap.MachineName, Bootstrap.SessionId); ;

                EventManager.LogETLJobEvent(eventArgs);
            }
        }
    }
}
