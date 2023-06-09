using FNBCoreETL.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB
{
    public static class Bootstrap
    {
        internal static string ApplicationName { get; private set; }
        internal static string MachineName { get; private set; }
        internal static string Environment { get; private set; }
        internal static Guid SessionId { get; private set; }
        internal static string FNBCustomConnStr { get; private set; }
        public static void Go(string applicationName, string machineName, string envrionment, Guid sessionId,
            string fnbCustomConnStr, VerbosityEnum loggingVerbosity)
        {
            ApplicationName = applicationName;
            MachineName = machineName;
            Environment = envrionment;
            SessionId = sessionId;
            FNBCustomConnStr = fnbCustomConnStr;
            EventManager.SubscribeToAppLog(loggingVerbosity, FNBTellerETLADODB.Logging.ApplicationLogger.Log);
            EventManager.SubscribeToJobLog(FNBTellerETLADODB.Logging.JobLogger.Log);
        }
    }
}
