using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Util
{
    //TODO: Review this, might not be best position
    public static class ServerLogUtil
    {
        private static EventLog _appEventLog;
        private static readonly string _serverLogType = "Application";

        /// <summary>
        /// Wont throw, returns true on success
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="eType"></param>
        public static bool TryWriteToServerLog(string msg, EventLogEntryType eType)
        {
            bool retVal = false;
            try
            {
                if (_appEventLog == null)
                {
                    if (!System.Diagnostics.EventLog.SourceExists(Bootstrap.ApplicationName))
                    {
                        System.Diagnostics.EventLog.CreateEventSource(
                            Bootstrap.ApplicationName, _serverLogType);
                    }
                    _appEventLog = new EventLog();
                    _appEventLog.Source = Bootstrap.ApplicationName;
                    _appEventLog.Log = _serverLogType;
                }

                _appEventLog.WriteEntry(msg, eType);
                retVal = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return retVal;
        }
    }
}
