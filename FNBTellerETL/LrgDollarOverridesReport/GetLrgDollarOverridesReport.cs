using FNBCoreETL.Framework;
using FNBCoreETL.Util;
using FNBTellerETL.Config;
using FNBTellerETL.Util;
using FNBTellerETLADODB.LrgDollarOverridesReport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FNBTellerETL.LrgDollarOverridesReport
{
    public static class GetLrgDollarOverridesReport
    {
        private static readonly string REPORT_NAME = "Large-Dollar-Overrides-" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv";
        private static readonly string REPORT_LOCATION = FileConfiguration.LargeDollarOverridesReport.ReportFileLocation.Value + REPORT_NAME;

        public static ETLJobOut Go(ETLJobIn job)
        {
            var ejExtractFileList = new List<string>();
            List<LargeDollarOverrideModel> overrideList;
            List<DateTime> dates;

            switch (job.ModeName)
            {
                case "Normal":
                    //get one weeks worth of data
                    var fromDate = DateTime.Now.AddDays(-7).Date;
                    var toDate = DateTime.Now.Date;

                    overrideList = FNBTellerETLADODB.LrgDollarOverridesReport.LrgDollarOverrides.Get(fromDate, toDate);
                    dates = GetDaysBetween(fromDate, toDate);
                    foreach (var date in dates)
                    {
                        var fpath = FileUtil.GetFileByName(FileConfiguration.EjExtractUtil.OutputXMLDir.Value, date.ToString("yyyyMMdd") + ".XML");
                        if (fpath != null)
                            ejExtractFileList.Add(fpath);
                    }
                    break;
                default:
                    throw new ApplicationException($"job.ModeName is not recognized: ${job.ModeName}");
            }

            var finalizedOverrideList = GetEjElementsMatchingOverrides(dates, ejExtractFileList, overrideList);

            var output = WriteToCsv(finalizedOverrideList);

            EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value,
                FileConfiguration.LargeDollarOverridesReport.SendReportsTo.ToArray(),
                "Large Dollar Overrides Report - " + FileConfiguration.Environment,
                $"See Attached File.\nReport Location: {REPORT_LOCATION}\nTotal Transactions in Report: {overrideList.Count}\n\n",
                REPORT_LOCATION);

            return new ETLJobOut(overrideList.Count, output);
        }

        /// <summary>
        /// Given a List of dates to search over, a List of Ej Extract Data files (produced from BPUEJFMT.exe), and a List of large dollar supervisor override transactions, this method
        /// returns a List of LargeDollarOverrideModel objects. The returned List has the data from the provided tranList plus additional data found in the Ej Extract data file.
        /// </summary>
        /// <param name="dates"></param>
        /// <param name="fnames">>Ej Extract files to search through</param>
        /// <param name="tranList">List of LargeDollarOverrideModel objects, each of which corresponds to a given supervisor override transaction</param>
        /// <returns></returns>
        public static List<LargeDollarOverrideModel> GetEjElementsMatchingOverrides(List<DateTime> dates, List<string> fnames, List<LargeDollarOverrideModel> tranList)
        {
            var finalList = new List<LargeDollarOverrideModel>();

            foreach (var date in dates)
            {
                //get transactions for the given date
                var transactions = tranList.Where(item => item.Procdate == date).ToList();

                //get ej extract file for the given date
                var fullFilePath = FileConfiguration.EjExtractUtil.OutputXMLDir.Value + date.ToString("yyyyMMdd") + ".XML";
                var fname = fnames.Where(filename => filename.Equals(fullFilePath)).FirstOrDefault();

                //if no transactions for a given date, or no file for a given date, go to next iteration
                if (transactions.Count == 0 || fname == null)
                    continue;

                finalList.AddRange(GetEjExtractElements(fname, transactions));
            }
            return finalList;
        }

        /// <summary>
        /// Given an Ej Extract file and a list of supervisor override transactions that occured on that day, this method
        /// returns a list of LargeDollarOverrideModel objects containing data from both the ARGOENT database and from the EJ Extract Utility
        /// </summary>
        public static List<LargeDollarOverrideModel> GetEjExtractElements(string fname, List<LargeDollarOverrideModel> transactions)
        {
            using (XmlReader reader = XmlReader.Create(fname))
            {
                if (reader.IsStartElement() && reader.Name.Equals("BPUEJFMT"))
                {
                    while (reader.Read())
                    {
                        //DEPMIC2 is deposit Node in Ej Extract data file
                        if (reader.Name.Equals("DEPMIC2"))
                        {
                            XElement el = XNode.ReadFrom(reader) as XElement;
                            var ejRegionId = el.Element("ADS_REGIONID").Value.Trim(' ');
                            var ejOfficeId = el.Element("ADS_OFFICEID").Value.Trim(' ');
                            var ejCashboxNum = el.Element("ADS_CASHID").Value.Trim(' ');
                            var ejTranSeqNum = el.Element("ADS_TRANSEQ").Value.Trim(' ');
                            var ejMsgSeqNum = el.Element("ADS_MSGSEQ").Value.Trim(' ');

                            var tranOfInterest = transactions.Where(tran => tran.RegionId.Trim(' ') == ejRegionId && tran.OfficeId.Trim(' ') == ejOfficeId
                                && tran.CashboxId.Trim(' ') == ejCashboxNum && tran.TranSeqNum.ToString().Trim(' ') == ejTranSeqNum && tran.MsgSeqNum.ToString().Trim(' ') == ejMsgSeqNum)
                                .FirstOrDefault();

                            if (tranOfInterest != null)
                                LrgDollarOverrides.SetMissingModelValues(tranOfInterest, el);
                        }

                        //OVRDET is override detail Node in Ej Extract data file
                        if (reader.Name.Equals("OVRDET"))
                        {
                            XElement el = XNode.ReadFrom(reader) as XElement;
                            var ejRegionId = el.Element("ADS_REGIONID").Value.Trim(' ');
                            var ejOfficeId = el.Element("ADS_OFFICEID").Value.Trim(' ');
                            var ejCashboxNum = el.Element("ADS_CASHID").Value.Trim(' ');
                            var ejTranSeqNum = el.Element("ADS_TRANSEQ").Value.Trim(' ');

                            var tranOfInterest = transactions.Where(tran => tran.RegionId.Trim(' ') == ejRegionId && tran.OfficeId.Trim(' ') == ejOfficeId
                                && tran.CashboxId.Trim(' ') == ejCashboxNum && tran.TranSeqNum.ToString().Trim(' ') == ejTranSeqNum)
                                .FirstOrDefault();

                            if (tranOfInterest != null)
                            {
                                LrgDollarOverrides.SetMissingModelValues(tranOfInterest, el);
                            }
                        }
                    }
                }
                else
                {
                    throw new ApplicationException($"Unexpected root node in {fname}");
                }
            }

            return transactions;
        }

        public static string WriteToCsv(List<LargeDollarOverrideModel> list)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Branch Number,Branch Name,Region Name,Cashbox Number,Teller Name,Deposit Date,Account Number,Customer Name,Total Deposit Amount,Transit Check Total in Deposit,Supervisor Override Approver Name,Ledger Balance Before Transaction,Ledger Balance After Transaction");

            foreach (var item in list)
            {
                item.CustomerName = item.CustomerName == null ? "?" : item.CustomerName.Replace(",", ".");
                item.TellerName = item.TellerName == null ? "?" : item.TellerName.Replace(",", ".");
                item.OverrideName = item.OverrideName == null ? "?" : item.OverrideName.Replace(",", ".");

                csv.AppendLine($"{item.OfficeId},{item.OfficeName},{item.RegionName},{item.CashboxId},{item.TellerName},{item.Procdate},{item.AcctNum ?? "?"},{item.CustomerName},{item.AmountOne ?? 0.0},{item.AmountTwo ?? 0.0},{item.OverrideName},{item.AvailableBalance ?? "?"},{item.CurrentBalance ?? "?"}");
            }

            File.WriteAllText(REPORT_LOCATION, csv.ToString());
            return csv.ToString();
        }

        public static List<DateTime> GetDaysBetween(DateTime fromDate, DateTime toDate)
        {
            var dates = new List<DateTime>();
            for (var dt = fromDate; dt <= toDate; dt = dt.AddDays(1))
            {
                dates.Add(dt.Date);
            }
            return dates;
        }
    }
}
