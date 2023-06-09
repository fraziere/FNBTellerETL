using FNBCoreETL.Framework;
using FNBCoreETL.Util;
using FNBTellerETL.Config;
using FNBTellerETLADODB.MIMonthlyMonitoringReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL.MIMonthlyMonitoringReport
{
    public static class GetMIMonthlyMonitoring
    {
        private static readonly string REPORT_NAME = "MIMonthlyMonitoringReport_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv";
        private static readonly string REPORT_LOCATION = FileConfiguration.MIMonthlyMonitoringReport.ReportFileLocation.Value + REPORT_NAME;

        public static ETLJobOut Go(ETLJobIn job)
        {
            List<MIMonthlyMonitoringModel> modelList;

            switch (job.ModeName)
            {
                case "Normal":
                    modelList = FNBTellerETLADODB.MIMonthlyMonitoringReport.GetMIMonthlyMonitoringInfo.Get();
                    break;
                default:
                    throw new ApplicationException($"job.ModeName is not recognized: ${job.ModeName}");
            }

            var output = WriteToCsv(modelList);


            EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value,
                            FileConfiguration.MIMonthlyMonitoringReport.SendReportsTo.ToArray(),
                            "MI Monthly Monitoring Report - " + FileConfiguration.Environment,
                            $"See Attached File.\nReport Location: {REPORT_LOCATION}\nTotal Rows in Report: {modelList.Count}\n\n",
                            REPORT_LOCATION);


            return new ETLJobOut(modelList.Count, output);
        }

        private static string WriteToCsv(List<MIMonthlyMonitoringModel> modelList)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Transaction Date, Transaction Code, Transaction Description, Serial Number, Amount, Funding Transaction Type," +
                           "Funding Account Type Code, Funding Account Number, Payee Name, Purchaser Name, Pur Customer Type, Pur Internal Type," +
                           "Funding Transaction Number, Transaction Number, Fee Amount, Fee Funded by Cash, Tin Type, Tin Descr, Tin Number, " +
                           "Recourse Acct Type Code, Recourse Acct Number, Recourse Transit, Teller Id, Teller Name, Branch Code");
            
            foreach (var model in modelList)
            {
                csv.AppendLine(model.ToString());
            }

            File.WriteAllText(REPORT_LOCATION, csv.ToString());
            return csv.ToString();
        }
    }
}
