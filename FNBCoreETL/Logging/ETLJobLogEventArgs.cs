using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBCoreETL.Logging
{
    public class ETLJobLogEventArgs : EventArgs
    {
        public bool IsError { get; set; }
        public SeverityEnum Severity { get; private set; }
        public string Environment { get; private set; }
        public string Command { get; private set; }
        public string CommandName { get; private set; }
        public string Mode { get; private set; }
        public string ModeName { get; private set; }
        public string Arguments { get; private set; }
        public string ShortMsg { get; private set; }
        public string LongMsg { get; private set; }
        public int? RecordCount { get; private set; }
        public double? RoundTripMs { get; private set; }
        public string Output { get; private set; }
        public string ServerName { get; private set; }
        public Guid SessionId { get; private set; }

        public ETLJobLogEventArgs(bool isError, SeverityEnum severity, string environment,
            string command, string commandName,
            string mode, string modeName, string arguments,
            string shortMsg, string longMsg,
            int? recordCount, double? roundTripMs,
            string output, string serverName, Guid sessionId)
        {
            this.IsError = isError;
            this.Severity = severity;
            this.Environment = environment;
            this.Command = command;
            this.CommandName = commandName;
            this.Mode = mode;
            this.ModeName = modeName;
            this.Arguments = arguments;
            this.ShortMsg = shortMsg;
            this.LongMsg = longMsg;
            this.RecordCount = recordCount;
            this.RoundTripMs = roundTripMs;
            this.Output = output;
            this.ServerName = serverName;
            this.SessionId = sessionId;
        }

    }
}
