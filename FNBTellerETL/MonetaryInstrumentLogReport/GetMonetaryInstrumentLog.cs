using FNBCoreETL.Framework;
using FNBCoreETL.Util;
using FNBTellerETL.Config;
using FNBTellerETLADODB.MonetaryInstrumentLogReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FNBTellerETL.MonetaryInstrumentLogReport
{
    public static class GetMonetaryInstrumentLogReport
    {
        private static readonly string REPORT_NAME = "MonetaryInstrumentLogReport_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv";
        private static readonly string REPORT_LOCATION = FileConfiguration.MonetaryInstrumentLogReport.ReportFileLocation.Value + REPORT_NAME;

        public static ETLJobOut Go(ETLJobIn job)
        {
            List<MonetaryInstrumentLogModel> modelList;

            switch (job.ModeName)
            {
                case "Normal":
                    modelList = FNBTellerETLADODB.MonetaryInstrumentLogReport.MonetaryInstrumentLogReport.Get();
                    break;
                default:
                    throw new ApplicationException($"job.ModeName is not recognized: ${job.ModeName}");
            }

            var output = WriteToCsv(modelList);

            EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value,
                            FileConfiguration.MonetaryInstrumentLogReport.SendReportsTo.ToArray(),
                            "Monetary Instrument Log Report - " + FileConfiguration.Environment,
                            $"See Attached File.\nReport Location: {REPORT_LOCATION}\nTotal Rows in Report: {modelList.Count}\n\n",
                            REPORT_LOCATION);

            return new ETLJobOut(modelList.Count, output);
        }

        private static string WriteToCsv(List<MonetaryInstrumentLogModel> modelList)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Transaction Date, Amount, Transaction Code, Office ID, Branch Code, Serial Number," +
                           "Purchaser, Pur Customer Type, Pur Address, Pur City, Pur State," +
                           "Pur ZipCode, Pur Country, Date Of Birth, ID Type, ID Descr, ID Number, ID State, ID Issuer, TIN Type, TIN Descr, TIN Number");

            foreach (var model in modelList)
            {
                csv.AppendLine(model.ToString());
            }

            File.WriteAllText(REPORT_LOCATION, csv.ToString());
            return csv.ToString();
        }
    }
}
