using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using FNBTellerETLADODB.ETLJobMonitor;
using FNBTellerETL.Config;
using FNBCoreETL.Util;

namespace FNBTellerETL.ETLJobMonitor
{
    /// <summary>
    ///  This tool emails the Developers a report of all Reports run that day
    ///  
    ///  This should be the last one called, or scheduled later in the morning (ex:4:55am)
    ///  
    ///  ETLMonitor method is the command / control one
    ///  DataExtractor is in FNBTellerETLADO
    ///     it should produce lists of the dbo.ReportStatus and etl.JobLog
    ///     (as C# objects in a list)
    /// </summary>
    public static class RunETLJobMonitor
    {
        private static bool wasETLFailure = false;

        public static ETLJobOut Go(ETLJobIn job)
        {
            //get data
            IDataExtractor _dataExtractor = new DataExtractor();
            var todaysDate = DateTime.Now.ToString("MM-dd-yyyy");
            var todaysJobLogList = _dataExtractor.JobLogToList(todaysDate);
            var scheduledJobs = GetTodaysJobs(_dataExtractor.GetScheduledJobs(), todaysDate);

            //proccess data
            var emailbody = GetFormulatedReport(todaysJobLogList, scheduledJobs);

            //Email out data
            EmailUtil.SendEmail(
                FileConfiguration.Email.SendProgramReportsFrom.Value,
                FileConfiguration.ETLJobMonitor.SendReportsTo.ToArray(),
                "ETLJobMonitor - " + FileConfiguration.Environment,
                emailbody,
                attachment: null);

            return new ETLJobOut(todaysJobLogList.Count(), emailbody);
        }

        private static string GetFormulatedReport(IEnumerable<JobLog> todaysJobLogList, IEnumerable<ReportStatus> scheduledJobs)
        {
            //split into missing & Success
            string successfulReports = "";
            string missingFailReports = "";

            foreach (var sJob in scheduledJobs)
            {
                if(!todaysJobLogList.Any(x => (x.command == sJob.command && x.commandName == sJob.commandName)))
                {
                    missingFailReports += $"{sJob.reportEnvironment} | ??? | {sJob.commandName} \t| ??? | Scheduled\n";
                }
            }


            foreach (var job in todaysJobLogList)
            {
                if (!job.isError && job.recordCount != -1)
                {
                    successfulReports += $"{job.environment} | {job.recordCount} | {job.commandName} \t| {job.createDate} | {(job.roundTripMs / 1000).Value.ToString("0.00")} Sec" + "\n";
                }
                else
                {
                    missingFailReports += $"{job.environment} | ??? | {job.commandName} \t| ???\n";
                }
            }

            string formulatedReport = "";
            if (missingFailReports == "")
            {
                formulatedReport = "SuccessfulRun";
                wasETLFailure = false;
            }
            else
            {
                formulatedReport = "Failure or Missing Report";
                wasETLFailure = true;
            }

            formulatedReport += "\n\n";

            formulatedReport += "--------------------\n";
            formulatedReport += successfulReports;
            formulatedReport += "--------------------\n";
            formulatedReport += missingFailReports;
            formulatedReport += "--------------------\n";

            return formulatedReport;
        }

        private static IEnumerable<ReportStatus> GetTodaysJobs(IEnumerable<ReportStatus> scheduledJobs, string todaysDate)
        {
            var returnVal = new List<ReportStatus>();

            returnVal.AddRange(scheduledJobs.Where(x => x.reportFrequency == "daily").ToList());

            if(DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                returnVal.AddRange(scheduledJobs.Where(x => x.reportFrequency == "weeklyMonday").ToList());
            }

            if(DateTime.Now.Day == 1)
            {
                returnVal.AddRange(scheduledJobs.Where(x => x.reportFrequency == "monthly1").ToList());
            }

            return returnVal;
        }

    }
}
