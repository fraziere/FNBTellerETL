using FNBCoreETL.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNBTellerETLADODB.TransitDepositReport;
using FNBTellerETL.Config;
using System.IO;
using System.Net.Mail;
using FNBCoreETL.Logging;
using FNBCoreETL.Util;

namespace FNBTellerETL.TransitDepositsReport
{
    public static class GetTransitDepositReport
    {
		private static readonly string REPORT_NAME = "Transit-Deposit-Report-" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv";
		private static readonly string REPORT_LOCATION = FileConfiguration.TransitDepositReport.ReportFileLocation.Value + REPORT_NAME;

		public static ETLJobOut Go(ETLJobIn job)
        {

			//TODO: Replace these SWAP to 3 Months
			var minDateRange = new DateTime(2021, 07, 01);
			var maxDateRange = new DateTime(2021, 08, 01);

			switch (job.ModeName)
            {
                case "Normal":
					minDateRange = DateTime.Now.AddMonths(-3);
					maxDateRange = DateTime.Now;
					break;
				case "PastMonth":
					minDateRange = DateTime.Now.AddMonths(-1);
					maxDateRange = DateTime.Now;
					break;
				case "Past7Days":
					minDateRange = DateTime.Now.AddDays(-7);
					maxDateRange = DateTime.Now;
					break;
				default:
                    throw new ApplicationException($"job.ModeName is not recognized: ${job.ModeName}");
            }

			//----------------------------------------------------------

			//Get Account Data (into DataTable) (6 Mill)
			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate1", "", null);

			//Get 1 Month of Data Into data table??? 40K per day
			var transactionData = GetTellerTransactionData.Get(minDateRange, maxDateRange); //ONLY DEPMIC2 (TranID equivilent)


			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate2", "transactionData Count: " + transactionData.Count().ToString(), null);

			var distinctAccountShortList = new List<string>();
			distinctAccountShortList.AddRange(transactionData.Select(x => x.acctNum.Trim().Replace("-", String.Empty)).Distinct());

			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate3", distinctAccountShortList.Count().ToString(), null);

			var accountData = GetCustomerAccountData.Get(distinctAccountShortList);

			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate4", accountData.Count().ToString(), null);
			//----------------------------------------------------------
			//Combine on tellerData output
			var combinedTellerTranCustAccountData = GetCombinedAccountTranData(accountData, transactionData);

			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate5", combinedTellerTranCustAccountData.Count().ToString(), null);
			//Fillter into new List TranSum and Counts

			var filteredList = GetTranSumAccountCombined(combinedTellerTranCustAccountData);

			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate6", filteredList.Count().ToString(), null);

			filteredList = filteredList.Where(x => x.totalAmountTransitChecks >= 5000m).ToList();

			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate7", filteredList.Count().ToString(), null);


			//output to CSV
			var outputString = OutputToCSV(filteredList);

			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate8", "OutputCSV Generated", null);

			EmailUtil.SendEmail(FileConfiguration.Email.SendProgramReportsFrom.Value,
							FileConfiguration.TransitDepositReport.SendReportsTo.ToArray(),
							"TransitDepositReport_Email - " + FileConfiguration.Environment,
							$"TransitDepositReport_Email\nTotal transit deposit rows in report:\t{filteredList.Count()}\n\n",
							REPORT_LOCATION);

			Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "TDR Gate9", "", null);

			return new ETLJobOut(filteredList.Count(), outputString);
        }


		private static string OutputToCSV(List<TranSumAccountcombined> list)
        {
			var csv = new StringBuilder();
			string header = "ProcDate,Region ID,Office ID,Office Name,CashBox,tranSeqNumGroup,AccountNumber,accountType,customerName," +
				"totalFNBDepositAmount,totalFNBNumberOfItemsInDeposit,totalNumberOnUsChecks,totalAmountOnUsChecks,totalNumberTransitChecks,totalAmountTransitChecks";

			csv.AppendLine(header);

			foreach (TranSumAccountcombined tranSumCombined in list)
			{
				var newCsvLine = $"{(tranSumCombined.procDate.ToString()) ?? ""},{tranSumCombined.regionId ?? ""},{tranSumCombined.officeId ?? ""},{tranSumCombined.officeName ?? ""},{tranSumCombined.cashBox ?? ""},{tranSumCombined.tranSeqNumGroup.ToString() ?? ""},{tranSumCombined.accountNumber ?? ""},{tranSumCombined.accountType ?? ""},{tranSumCombined.customerName ?? ""}" +
					$",{tranSumCombined.totalFNBDepositAmount},{tranSumCombined.totalFNBNumberOfItemsInDeposit},{tranSumCombined.totalNumberOnUsChecks},{tranSumCombined.totalAmountOnUsChecks},{tranSumCombined.totalNumberTransitChecks},{tranSumCombined.totalAmountTransitChecks}";
				csv.AppendLine(newCsvLine);
			}

			File.WriteAllText(REPORT_LOCATION, csv.ToString());
			return csv.ToString();
		}

		private static List<CombinedAccountTranData> GetCombinedAccountTranData(List<CustomerAccountModel> customerAccountsIn, List<TellerTranModel> tellerTrans)
        {
			var returnVal = new List<CombinedAccountTranData>();
			var customerAccounts = new Dictionary<string, List<CustomerAccountModel>>();

			foreach(var account in customerAccountsIn)
            {
				if (customerAccounts.ContainsKey(account.acct.Trim()))
                {
					if (!customerAccounts[account.acct.Trim()].Any(x => x.acct.Trim() == account.acct.Trim() && 
																x.customerId == account.customerId && 
																x.fullName == account.fullName))
					{
						customerAccounts[account.acct.Trim()].Add(account);
					}
				}
                else
                {
					customerAccounts.Add(account.acct.Trim(), new List<CustomerAccountModel>());
					customerAccounts[account.acct.Trim()].Add(account);
				}
            }
			
			foreach(var tranInfo in tellerTrans)
            {
				var holder = new CombinedAccountTranData();
				holder.tranInfo = tranInfo;
                if (customerAccounts.ContainsKey(tranInfo.acctNum.Trim().Replace("-", String.Empty)))
                {
					holder.customerInfo = customerAccounts[tranInfo.acctNum.Trim().Replace("-", String.Empty)].FirstOrDefault();
                }
				returnVal.Add(holder);
			}

			return returnVal;
        }

		private static List<TranSumAccountcombined> GetTranSumAccountCombined(List<CombinedAccountTranData> combinedAccountTranData)
        {
			var returnVal = new List<TranSumAccountcombined>();
			//var returnVal = new Dictionary<int, TranSumAccountcombined>();

			foreach (var item in combinedAccountTranData)
            {
				//add stuff to existing
				if (returnVal.Exists(x => x.procDate == item.tranInfo.procDate 
								&& x.regionId == item.tranInfo.regionID
								&& x.officeId == item.tranInfo.branchNumber
								&& x.cashBox == item.tranInfo.cashboxNumber
								&& x.tranSeqNumGroup == item.tranInfo.transactionSumNum
								&& x.accountNumber == item.tranInfo.acctNum))
                {
					//AddDuplicates
					returnVal.Where(x => x.procDate == item.tranInfo.procDate
								&& x.regionId == item.tranInfo.regionID
								&& x.officeId == item.tranInfo.branchNumber
								&& x.cashBox == item.tranInfo.cashboxNumber
								&& x.tranSeqNumGroup == item.tranInfo.transactionSumNum
								&& x.accountNumber == item.tranInfo.acctNum).ToList().ForEach(cc => {
									cc.totalFNBDepositAmount += item.tranInfo.amount;
									cc.totalFNBNumberOfItemsInDeposit += 1;
									cc.totalNumberOnUsChecks = isOnUsCheck(item.tranInfo.itemnumber) ? cc.totalNumberOnUsChecks + 1 : cc.totalNumberOnUsChecks;
									cc.totalAmountOnUsChecks = isOnUsCheck(item.tranInfo.itemnumber) ? cc.totalAmountOnUsChecks + item.tranInfo.amount : cc.totalAmountOnUsChecks;
									cc.totalNumberTransitChecks = isTransitCheck(item.tranInfo.itemnumber) ? cc.totalNumberTransitChecks + 1 : cc.totalNumberTransitChecks;
									cc.totalAmountTransitChecks = isTransitCheck(item.tranInfo.itemnumber) ? cc.totalAmountTransitChecks + item.tranInfo.amount : cc.totalAmountTransitChecks;
								});
				}
				else
                {
					var holder = new TranSumAccountcombined()
					{
						procDate = item.tranInfo.procDate,
						regionId = item.tranInfo.regionID,
						officeId = item.tranInfo.branchNumber,
						officeName = item.tranInfo.officeName,
						cashBox = item.tranInfo.cashboxNumber,
						tranSeqNumGroup = item.tranInfo.transactionSumNum,
						accountNumber = item.tranInfo.acctNum,
						accountType = item.customerInfo.description,
						customerName = (item.customerInfo.fullName ?? "").Replace(",", "."),
						totalFNBDepositAmount = item.tranInfo.amount,
						totalFNBNumberOfItemsInDeposit = 1,
						totalDepositAmountDebit = item.tranInfo.debitAmount,
						totalNumberOfItemsInDepositDebit = item.tranInfo.debitCount,
						totalDepositAmmountCredit = item.tranInfo.creditAmount,
						totalNumberOfItemsInDepositCredit = item.tranInfo.creditCount,
						totalNumberOnUsChecks = isOnUsCheck(item.tranInfo.itemnumber) ? 1 : 0,
						totalAmountOnUsChecks = isOnUsCheck(item.tranInfo.itemnumber) ? item.tranInfo.amount : 0,
						totalNumberTransitChecks = isTransitCheck(item.tranInfo.itemnumber) ? 1 : 0,
						totalAmountTransitChecks = isTransitCheck(item.tranInfo.itemnumber) ? item.tranInfo.amount : 0,
					};

					returnVal.Add(holder);
					//returnVal.Add(holder.GetHashCode(), holder);
				}
			}

			return returnVal;
			//return returnVal.Values.ToList();
		}

		private static bool isTransitCheck(int itemCode)
        {
			if (itemCode == 1091)
				return true;
			return false;
        }

		private static bool isOnUsCheck(int itemCode)
		{
			if (itemCode == 1041 || itemCode == 1031)
				return true;
			return false;
		}
	}

	
	internal class TranSumAccountcombined
    {

        public DateTime procDate { get; set; }
		public string regionId { get; set; }
		public string officeId { get; set; }
		public string officeName { get; set; }
		public string cashBox { get; set; }
		internal int tranSeqNumGroup { get; set; }
		public string accountNumber { get; set; }
		public string accountType { get; set; }
		public string customerName { get; set; }
		public decimal totalFNBDepositAmount { get; set; } //ARGO Number
		public int totalFNBNumberOfItemsInDeposit { get; set; } //ARGO Number
		public double totalDepositAmountDebit { get; set; } //ARGO Number
		public int totalNumberOfItemsInDepositDebit { get; set; } //ARGO Number

		public double totalDepositAmmountCredit { get; set; } //ARGO Number
		public int totalNumberOfItemsInDepositCredit { get; set; } //ARGO Number
																  //Transit OnUs Flag ???


		public int totalNumberOnUsChecks { get; set; }
		public decimal totalAmountOnUsChecks { get; set; }
		public int totalNumberTransitChecks { get; set; }
		public decimal totalAmountTransitChecks { get; set; }


		//averages????
	}

	internal struct CombinedAccountTranData
    {
		public CustomerAccountModel customerInfo { get; set; }
		public TellerTranModel tranInfo { get; set; }
	}
}
