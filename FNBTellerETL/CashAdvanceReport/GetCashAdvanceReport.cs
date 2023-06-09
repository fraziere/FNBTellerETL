using FNBCoreETL.Framework;
using FNBCoreETL.Util;
using FNBTellerETL.Config;
using FNBTellerETL.Util;
using FNBTellerETLADODB.CashAdvanceReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FNBTellerETL.CashAdvanceReport
{

    public static class GetCashAdvanceReport
    {
        private static readonly string REPORT_NAME = "Cash-Advance-Report-" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv";
        private static readonly string REPORT_LOCATION = FileConfiguration.CashAdvanceReports.ReportFileLocation.Value + REPORT_NAME;

        /// <summary>
        /// Performs all the actions to generate Argo Teller Cash Advance Report.
        /// Normal run includes data from previous day. DateRange run includes data for a range of dates.
        /// </summary>
        /// <param name="job">ETL Cash Advance job which contains the necessary modes and DateTime arguments</param>
        /// <returns>ETLJobOut</returns>
        public static ETLJobOut Go(ETLJobIn job)
        {
            List<CashAdvanceReportModel> data;
            switch (job.ModeName)
            {
                case "Normal":
                    data = FNBTellerETLADODB.CashAdvanceReport.GetCashAdvanceReport.Get(DateTime.Now.AddDays(-1).Date, DateTime.Now);
                    break;
                case "DateRange":
                    DateTime fromDate = DateTime.Parse(job.ArgumentsByName["Begin"]);
                    DateTime toDate = DateTime.Parse(job.ArgumentsByName["End"]);

                    //if dates are entered in backwards flip them and assume that was a mistake
                    if (fromDate > toDate)
                    {
                        var tempDate = fromDate;
                        fromDate = toDate;
                        toDate = tempDate;
                    }

                    data = FNBTellerETLADODB.CashAdvanceReport.GetCashAdvanceReport.Get(fromDate, toDate);
                    break;
                default:
                    throw new ApplicationException($"job.ModeName is not recognized: ${job.ModeName}");
            }

            decimal totalAmt = ComputeTransactionTotalAmt(data);
            string reportContents = WriteToCsv(data, totalAmt);

            string[] toAddresses = StrUtil.SemiColonDelimited(FileConfiguration.CashAdvanceReports.SendReportsTo.Value).ToArray();
            EmailUtil.SendEmail(
                FileConfiguration.Email.SendProgramReportsFrom.Value,
                toAddresses,
                $"Cash Advance Report - {FileConfiguration.Environment}",
                $"See Attached File\nReport Location: {REPORT_LOCATION}\nTotal Transactions in Report: {data.Count}\n\n",
                REPORT_LOCATION
                );

            return new ETLJobOut(data.Count, reportContents);
        }

        internal static string WriteToCsv(List<CashAdvanceReportModel> list, decimal total)
        {
            var csv = new StringBuilder();
            string header = "Process Date, Cost Center Number, Branch Name, Amount";
            csv.AppendLine(header);

            foreach(CashAdvanceReportModel cashAdvTrans in list)
            {
                var newCsvLine = $"{cashAdvTrans.ProcessingDate}, {cashAdvTrans.OfficeId}, {cashAdvTrans.OfficeName}, {cashAdvTrans.TotalAmount}";
                csv.AppendLine(newCsvLine);
            }
            csv.AppendLine($"Report Total, {null}, {null}, {total}");

            File.WriteAllText(REPORT_LOCATION, csv.ToString());
            return csv.ToString();
        }

        /// <summary>
        /// Returns total of all Cash Advance transactions. Any Cash Advance amount which equals null in the input List will be treated as 0.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static decimal ComputeTransactionTotalAmt(List<CashAdvanceReportModel> list)
        {
            decimal total = 0;
            foreach(CashAdvanceReportModel cashAdvTrans in list)
            {
                //if cashAdvTrans.TotalAmount is null, say it's 0
                total = (cashAdvTrans.TotalAmount ?? 0) + total;
            }
            return total;
        }
    }
}
