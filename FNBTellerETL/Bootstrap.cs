using FNBCoreETL.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL
{
    public static class Bootstrap
    {
        internal static string ApplicationName { get; private set; }
        internal static string MachineName { get; private set; }
        internal static string Environment { get; private set; }
        internal static Guid SessionId { get; private set; }
        public static void Go(string applicationName, string machineName,
            string environment, Guid sessionId, string fnbCustomConnStr, VerbosityEnum loggingVerbosity)
        {

            ApplicationName = applicationName;
            MachineName = machineName;
            Environment = environment;
            SessionId = sessionId;
            FNBCoreETL.Bootstrap.Go(applicationName, machineName, environment, sessionId);
            FNBTellerETLADODB.Bootstrap.Go(applicationName, machineName, environment, sessionId, fnbCustomConnStr, loggingVerbosity);
        }

    }
}
