using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using FNBTellerETL.Config;
using System.IO;
using FNBCoreETL.Util;
using FNBTellerETL.Util;
using System.Text.RegularExpressions;

namespace FNBTellerETL.AmlAccountUpdater
{
    public static class SetAmlAccounts
    {
        private static readonly string CTR_EXEMPTION_ACCT_FILE = FileConfiguration.AmlAccountUpdater.InputFileDir.Value
            + "ctr_exemption_flag_data_file_"
            + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
        private static readonly string FILENAME_PATTERN = "ctr_exemption_flag_data_file_";

        public static ETLJobOut Go(ETLJobIn job)
        {
            List<string> accountsUpdated = new List<string>();
            FNBTellerETLADODB.AmlAccountsUpdater.SetAmlAccounts.SetAll();

            switch (job.ModeName)
            {
                case "Normal":
                    {
                        List<string> accountsToBeUpdated = ParseCsvData(CTR_EXEMPTION_ACCT_FILE);

                        if (accountsToBeUpdated.Count != 0)
                        {
                            //update ctr flag for all accounts in list (if account exists in AML_ACCOUNT table)
                            accountsUpdated = FNBTellerETLADODB.AmlAccountsUpdater.SetAmlAccounts.Set(accountsToBeUpdated);
                        }
                        break;
                    }
                case "DateRange":
                    {
                        DateTime afterDate = DateTime.Parse(job.ArgumentsByName["AfterDate"]);
                        foreach (var file in Directory.EnumerateFiles(FileConfiguration.AmlAccountUpdater.InputFileDir.Value))
                        {
                            //if file was created after specified date and filename matches the ctr_exemption file naming convention
                            if ((File.GetCreationTime(file) >= afterDate) && Regex.IsMatch(file, FILENAME_PATTERN))
                            {
                                List<string> accountsToBeUpdated = ParseCsvData(file);
                                if(accountsToBeUpdated.Count != 0)
                                {
                                    accountsUpdated.AddRange(FNBTellerETLADODB.AmlAccountsUpdater.SetAmlAccounts.Set(accountsToBeUpdated));
                                }
                            }
                        }
                        break;
                    }
                default:
                    throw new ApplicationException($"job.ModeName is not recognized: {job.ModeName}");
            }

            FileUtil.MoveFile(CTR_EXEMPTION_ACCT_FILE, FileConfiguration.AmlAccountUpdater.HistoryDir.Value);

            string[] toAddresses = StrUtil.SemiColonDelimited(FileConfiguration.AmlAccountUpdater.SendJobAlertsTo.Value).ToArray();
            EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value, 
                            toAddresses, 
                            $"{job.CommandName} - {FileConfiguration.Environment} Completed Successfully", 
                            $"Total CTR Flags updated: {accountsUpdated.Count}\n\n",
                            attachment: null);

            //creates output string for logging. Each account updated will be on newline
            string output = string.Join(Environment.NewLine, accountsUpdated.ToArray());

            return new ETLJobOut(accountsUpdated.Count, output);
        }

        /// <summary>
        /// Returns a list of accounts extracted from a CSV file. If CSV file doesn't exist an exeption is thrown.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns>List of account numbers</returns>
        internal static List<string> ParseCsvData(string filepath)
        {
            if (!File.Exists(filepath))
            {
                string[] toAddresses = StrUtil.SemiColonDelimited(FileConfiguration.AmlAccountUpdater.SendJobAlertsTo.Value).ToArray();

                EmailUtil.SendEmail(
                    FileConfiguration.Email.SendProgramReportsFrom.Value,
                    toAddresses,
                    $"Missing CTR Exemption File - {FileConfiguration.Environment}",
                    $"Could not find file: {filepath}\n\n",
                    attachment: null);

                throw new ApplicationException($"Today's AML account file does not exist: {filepath}");
            }

            List<string> accountList = new List<string>();

            using (var reader = new StreamReader(filepath))
            {
                int lineCount = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    char[] charsToTrim = { '"', ' ' };
                    line = line.Trim(charsToTrim);

                    if (lineCount > 0)
                    {
                        //remove leading space before account numbers. For Ex 123| 1234 -> 123|1234
                        line = line.Replace(" ", "");
                        //Seperate each Account Type and Number to a seperate line
                        var accountArr = line.Split('|');
                        accountList.AddRange(accountArr);
                    }
                    lineCount++;
                }
            }

            return accountList;
        }

    }
}
