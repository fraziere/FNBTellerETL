using FNBCoreETL.Framework;
using FNBCoreETL.Util;
using FNBTellerETL.Config;
using FNBTellerETL.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FNBTellerETL.OfficalCheckReport
{
    public static class GetOfficialCheckReport
    {
        private static string csvFileName;
        private static List<FNBTellerETLADODB.OfficialCheckReport.OfficalCheckModel> data;

        /// <summary>
        /// Performs all the actions to generate Official Check Report.
        /// Normal run includes data from previous day. DateRange run includes data for a range of dates.
        /// </summary>
        /// <param name="job">ETL Official Check job which contains the necessary modes and DateTime arguments</param>
        /// <returns>ETLJobOut</returns>
        public static ETLJobOut Go(ETLJobIn job)
        {
            //set as midnight to 1 second before midnight (all of that day inclusive)
            DateTime today = DateTime.Now.Date.AddSeconds(-1);
            DateTime yesterday = DateTime.Now.Date.AddDays(-1);

            string csvOutputText;
            csvFileName = "OfficalCheckReport_" + yesterday.ToString("yyyyMMdd");

            switch (job.ModeName)
            {
                case "Normal":
                    csvFileName = "OfficalCheckReport_" + yesterday.ToString("yyyyMMdd");
                    data = FNBTellerETLADODB.OfficialCheckReport.GetOfficalCheckInfo.Get(yesterday, today);
                    break;
                case "DateRange":

                    var formatedFromToDate = DateUtil.formatFromToOrder(job.ArgumentsByName["Begin"], job.ArgumentsByName["End"]);
                    DateTime fromDate = formatedFromToDate.fromDateOut;
                    DateTime toDate = formatedFromToDate.toDateOut;

                    csvFileName = "OfficalCheckReport_" + fromDate.ToString("yyyyMMdd") + "-" + toDate.ToString("yyyyMMdd");
                    data = FNBTellerETLADODB.OfficialCheckReport.GetOfficalCheckInfo.Get(fromDate, toDate);
                    break;
                default:
                    data = null;
                    break;
            }

            csvOutputText = OutputToCSV(data);

            EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value,
                FileConfiguration.OfficalCheckReport.SendReportsTo.ToArray(),
                "OfficialCheckReport_Email - " + FileConfiguration.Environment,
                "OfficialCheckReport_Email\nTotal checks in report:\t" + data.Count() + "\n\n\n",
                FileConfiguration.OfficalCheckReport.ReportFileLocation.Value + "\\" + csvFileName + ".csv");

            return new ETLJobOut(data.Count, csvOutputText);
        }

        private static string OutputToCSV(IEnumerable<FNBTellerETLADODB.OfficialCheckReport.OfficalCheckModel> dataList)
        {
            var csv = new StringBuilder();
            csv.AppendLine($"RegionID,BranchNumber,CashboxNumber,Oper_id,EmployeeName,LimitsProfDesc,OperSecProfDesc,Override TypeFlag,Override ID,Overrider Name,Overrider Limits," +
                $"Itemnumber,LikelyPaymentType?,serial,field6,abanumber,field4,account,trancode," +
                $"amount,OfficeName,procdate,transeq,sequence,ISN");

            foreach (var item in dataList)
            {
                csv.AppendLine($"{item.regionID},{item.branchNumber},{item.cashboxNumber},{item.operId},{item.employeeName},{item.sysLimitsProf},{item.sysOperSecProf},{item.overrideTypeFlag},{item.overrideId},{item.overriderName},{item.overriderLimits}," +
                    $"{item.itemnumber},{CheckNumberLookup.GetPaymentType(item.itemnumber.ToString())},{item.serial},{item.field6},{item.abanumber},{item.field4},{item.account},{item.trancode}," +
                    $"{item.ammount},{item.officeName},{item.procDate},{item.transeq},{item.sequence},{item.ISN}");
            }

            File.WriteAllText(FileConfiguration.OfficalCheckReport.ReportFileLocation.Value + "\\" + csvFileName + ".csv", csv.ToString());
            return csv.ToString();
        }
    }
}
