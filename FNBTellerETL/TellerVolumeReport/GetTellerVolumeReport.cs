using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using FNBTellerETLADODB.TellerVolumeReport;
using System.Linq;
using System.Text;
using System.IO;
using FNBTellerETL.Config;
using System.Net.Mail;
using FNBCoreETL.Util;

namespace FNBTellerETL.TellerVolumeReport
{
    public static class GetTellerVolumeReport
    {
        private static readonly string REPORT_NAME = "Teller-Volume-Report-" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv";
        private static readonly string BRANCH_REPORT_NAME = "Branch-Volume-Report-" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv";
        private static readonly string REPORT_LOCATION = FileConfiguration.TellerVolumeReport.ReportFileLocation.Value + REPORT_NAME;
        private static readonly string BRANCH_REPORT_LOCATION = FileConfiguration.TellerVolumeReport.ReportFileLocation.Value + BRANCH_REPORT_NAME;

        public static ETLJobOut Go(ETLJobIn job)
        {
            DateTime endDateRange;
            DateTime midDateRange;
            DateTime startDateRange;

            switch (job.ModeName)
            {
                case "Normal":
                    startDateRange = new DateTime(DateTime.Now.AddDays(5).Year, DateTime.Now.AddDays(5).Month, 1);  //first day of Next Month (at midnight)
                    midDateRange = new DateTime(startDateRange.Year, startDateRange.Month, 1).AddMonths(-1).Date;   //first day of month
                    endDateRange = new DateTime(startDateRange.Year, startDateRange.Month, 1).AddMonths(-2).Date;   //first day of previous month
                    break;
                default:
                    throw new ApplicationException($"job.ModeName is not recognized: {job.ModeName}");
            }

            var thisMonthData = GetTellerVolume.Get(midDateRange.AddTicks(-1), startDateRange); //set to 11:59:59PM.....
            var lastMonthData = GetTellerVolume.Get(endDateRange.AddTicks(-1), midDateRange);

            SetLastMonthTransactionCount(thisMonthData, lastMonthData);
            SetThisMonthTransactionCount(thisMonthData, lastMonthData);

            
            thisMonthData.OrderBy(x => x.regionID).ThenByDescending(x => x.branchNumber).ThenByDescending(x => x.cashboxNumber);
            var outPutString = WriteTellerToCsv(thisMonthData);

            var thisMonthBranchData = GetBranchTotalReport(thisMonthData);
            WriteBranchToCsv(thisMonthBranchData);

            EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value,
                            FileConfiguration.TellerVolumeReport.SendReportsTo.ToArray(),
                            "TellerVolumeReport_Email - " + FileConfiguration.Environment,
                            $"TellerVolumeReport_Email\nTotal teller volume rows in report:\t{thisMonthData.Count()}\n\nTotal branch volume rows in report:\t{thisMonthBranchData.Count()}\n\n",
                            new string[] { REPORT_LOCATION, BRANCH_REPORT_LOCATION } );

            return new ETLJobOut(thisMonthData.Count(), outPutString);
        }

        internal static string WriteTellerToCsv(List<TellerVolumeModel> list)
        {
            var csv = new StringBuilder();
            string header = "Region, Region ID, Branch Name, Branch Number, Cashbox Number, Teller Name, Lobby Transcations, Drive-Up Transactions, Unspecified Transactions, Total This Month Transactions, Total Last Month Transactions";
            csv.AppendLine(header);

            foreach (TellerVolumeModel tellerVolumeItem in list)
            {
                var newCsvLine = $"{tellerVolumeItem.regionName}, {tellerVolumeItem.regionID}, {tellerVolumeItem.branchName}, {tellerVolumeItem.branchNumber}, {tellerVolumeItem.cashboxNumber}, {tellerVolumeItem.tellerName.Replace(",",".").Replace("’", "'")}, " +
                    $"{tellerVolumeItem.lobbyTranscations}, {tellerVolumeItem.driveUpTransactions}, {tellerVolumeItem.unspecifiedTransactions}, " +
                    $"{tellerVolumeItem.totalThisMonthTransactions}, {tellerVolumeItem.totalLastMonthTransactions}";
                csv.AppendLine(newCsvLine);
            }

            File.WriteAllText(REPORT_LOCATION, csv.ToString());
            return csv.ToString();
        }

        internal static string WriteBranchToCsv(List<BranchVolumeModel> list)
        {
            var csv = new StringBuilder();
            string header = "Region ID, Branch Name, Branch Number, Total This Month Transactions, Total Last Month Transactions";
            csv.AppendLine(header);

            foreach (BranchVolumeModel branchVolumeItem in list)
            {
                var newCsvLine = $"{branchVolumeItem.regionID}, {branchVolumeItem.branchName}, {branchVolumeItem.branchNumber}, " +
                    $"{branchVolumeItem.totalThisMonthTransactions}, {branchVolumeItem.totalLastMonthTransactions}";
                csv.AppendLine(newCsvLine);
            }

            File.WriteAllText(BRANCH_REPORT_LOCATION, csv.ToString());
            return csv.ToString();
        }

        /// <summary>
        /// Given a list of last months and this months teller transactions, this method will add tellers to the list of transactions
        /// a teller has completed this month. Tellers added to the list are tellers who did NOT perform any transactions 
        /// at the SAME BRANCH and used the SAME CASHBOX as the previous month (therefore their this month totals are 0)
        /// </summary>
        /// <param name="thisMonth"></param>
        /// <param name="lastMonth"></param>
        private static void SetThisMonthTransactionCount(List<TellerVolumeModel> thisMonth, List<TellerVolumeModel> lastMonth)
        {
            foreach (var lastMonthItem in lastMonth)
            {
                //is last month in this month?
                var thisMonthItem = thisMonth.Where(x => x.branchNumber == lastMonthItem.branchNumber && x.cashboxNumber == lastMonthItem.cashboxNumber && x.tellerName == lastMonthItem.tellerName)
                    .FirstOrDefault();

                if (thisMonthItem == null)
                {
                    thisMonth.Add(new TellerVolumeModel()
                    {
                        regionName = lastMonthItem.regionName,
                        regionID = lastMonthItem.regionID,
                        branchName = lastMonthItem.branchName,
                        branchNumber = lastMonthItem.branchNumber,
                        cashboxNumber = lastMonthItem.cashboxNumber,
                        tellerName = lastMonthItem.tellerName,
                        lobbyTranscations = 0,
                        driveUpTransactions = 0,
                        unspecifiedTransactions = 0,
                        totalThisMonthTransactions = 0,
                        totalLastMonthTransactions = lastMonthItem.totalThisMonthTransactions
                    });
                }
            }
        }

        /// <summary>
        /// Given a list of last months and this months teller transactions, this method will set the total number of transactions
        /// a teller has completed the previous month for tellers who were at the SAME BRANCH and using the SAME CASHBOX as the previous month
        /// </summary>
        /// <param name="thisMonth"></param>
        /// <param name="lastMonth"></param>
        private static void SetLastMonthTransactionCount(List<TellerVolumeModel> thisMonth, List<TellerVolumeModel> lastMonth)
        {
            foreach (var thisMonthItem in thisMonth)
            {
                var lastMonthItem = lastMonth.Where(x => x.branchNumber == thisMonthItem.branchNumber && x.cashboxNumber == thisMonthItem.cashboxNumber && x.tellerName == thisMonthItem.tellerName)
                    .FirstOrDefault();
                if (lastMonthItem == null)
                {
                    thisMonthItem.totalLastMonthTransactions = 0;
                }
                else
                {
                    thisMonthItem.totalLastMonthTransactions = lastMonthItem.totalThisMonthTransactions;
                }
            }
        }

        private static List<BranchVolumeModel> GetBranchTotalReport(List<TellerVolumeModel> filledOutTellerVolume)
        {
            var outputBranchList = new List<BranchVolumeModel>();

            foreach(var item in filledOutTellerVolume)
            {
                if (outputBranchList.Any(x => x.regionID == item.regionID && x.branchNumber == item.branchNumber)){
                    outputBranchList.Where(x => (x.branchNumber == item.branchNumber && x.regionID == item.regionID)).ToList().ForEach
                        (x =>
                        {
                            x.totalThisMonthTransactions += item.totalThisMonthTransactions;
                            x.totalLastMonthTransactions += item.totalLastMonthTransactions;
                        });
                }
                else
                {
                    var holder = new BranchVolumeModel();
                    holder.regionID = item.regionID;
                    holder.branchNumber = item.branchNumber;                    
                    holder.branchName = item.branchName;
                    holder.totalThisMonthTransactions = item.totalThisMonthTransactions;
                    holder.totalLastMonthTransactions = item.totalLastMonthTransactions;
                    outputBranchList.Add(holder);
                }
            }
            return outputBranchList;
        }
    }
}
