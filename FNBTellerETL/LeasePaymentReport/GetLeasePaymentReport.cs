using FNBCoreETL.Framework;
using FNBCoreETL.Util;
using FNBTellerETL.Config;
using FNBTellerETL.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FNBTellerETL.LeasePaymentReport
{

    /// <summary>
    ///  Generates Lease Payment Report
    ///  NOTE: Behaves diffrently in DEV vs LCL
    ///  Order:
    ///       Go -> GetLeaseBlobPayment -> Outputs list of Records for Range
    ///       Output to CSV -> emailed out CSV
    ///       Return ETL job with CSV as a string
    /// </summary>

    public static class GetLeasePaymentReport
    {
        private static string csvFileName;
        private static List<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportFormatedModel> data;

        public static ETLJobOut Go(ETLJobIn job)
        {
            var yesterday = DateTime.Now.AddDays(-1);
            string csvOutputText;
            csvFileName = "LeasePayReport_" + yesterday.ToString("yyyyMMdd");

            switch (job.ModeName)
            {
                case "Normal":
                    csvFileName = "LeasePayReport_" + yesterday.ToString("yyyyMMdd");
                    data = GetLeasePaymentsBlobData.Get(yesterday);
                    break;
                case "DateRange":
                    var formatedFromToDate = DateUtil.formatFromToOrder(job.ArgumentsByName["Begin"], job.ArgumentsByName["End"]);
                    DateTime fromDate = formatedFromToDate.fromDateOut;
                    DateTime toDate = formatedFromToDate.toDateOut;

                    csvFileName = "LeasePayReport_" + fromDate.ToString("yyyyMMdd") + "-" + toDate.ToString("yyyyMMdd");
                    data = GetLeasePaymentsBlobData.Get(fromDate, toDate);
                    break;
                default:
                    data = null;
                    break;
            }

            csvOutputText = OutputToCSV(data);

            if (FileConfiguration.Environment.Value != "LCL")
            {
                EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value,
                    FileConfiguration.LeasePaymentReport.SendReportsTo.ToArray(),
                    "LeasePaymentReport_Email - " + FileConfiguration.Environment,
                    "LeasePaymentReport_Email\nTotal lease payments in report:\t" + data.Count() + "\n\n\n",
                    FileConfiguration.LeasePaymentReport.OutputCSVDir.Value + "\\" + csvFileName + ".csv"
                    );
            }

            return new ETLJobOut(data.Count, csvOutputText);
        }

        private static string OutputToCSV(IEnumerable<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportFormatedModel> dataList)
        {
            var csv = new StringBuilder();
            csv.AppendLine($"TranSeqSummeryNum,TranSeq,Lease Customer Account Number,Customer Name,Processing Date,ISN,GUID,DrCr,Payment Amount,Branch Number,Employee Name,Cashbox Number,ItemNumber,Serial Number,field6,Tran Code,field4,PaymentType?");

            foreach (var item in dataList)
            {
                //csv.AppendLine($"{item.UtilCustAccountNumber},{item.UtilLeaCustomerName},{item.ProcDate},{item.Amount}, {item.ARGOAmount}, {item.Office},{item.NAME},{item.Cashbox},{item.CheckNum}");
                csv.AppendLine($"{item.transeqSummeryNum},{item.TranSeq},{item.UtilCustAccountNumber},{item.UtilLeaCustomerName.Replace(",",".")},{item.ProcDate},{item.ISN},{item.GUID},{item.CRDR},{item.Amount},{item.Office},{item.NAME},{item.Cashbox},{item.itemnumber},{item.serial},{item.field6},{item.trancode},{item.field4},{CheckNumberLookup.GetPaymentType(item.itemnumber)}");
            }

            File.WriteAllText(FNBTellerETL.Config.FileConfiguration.LeasePaymentReport.OutputCSVDir.Value + "\\" + csvFileName + ".csv", csv.ToString());
            return csv.ToString();
        }
    }
}

