using FNBCoreETL.Framework;
using FNBCoreETL.Model;
using FNBTellerETL.AmlAccountUpdater;
using FNBTellerETL.CashAdvanceReport;
using FNBTellerETL.LeasePaymentReport;
using FNBTellerETL.FileCleanupTool;
using FNBTellerETL.ETLJobMonitor;
using FNBTellerETL.OfficalCheckReport;
using System;
using FNBTellerETL.TellerVolumeReport;
using FNBTellerETL.Util;
using FNBTellerETL.LrgDollarOverridesReport;
using FNBTellerETL.TransitDepositsReport;
using FNBTellerETL.MonetaryInstrumentLogReport;
using FNBTellerETL.MIMonthlyMonitoringReport;

namespace FNBTellerETL
{
    public class EntryPoint : ETLFramework
    {
        //Try to make a common set of modes.  
        //Most ETL Jobs should (in theory) have common patterns/needs
        public enum Modes
        {
            Normal,
            DateRange,
            Past30Days,
            Past7Days,
            PastMonth
        }

        public override void BuildModel(ICommand model)
        {
            //IMPORTANT! Do not wrap anything in try/catch.
            //Let Exceptions bubble up to the framework.

            #region Instructions

            //mode/command pair uniqueness is CASE INSENTIVE and ALPHA-NUMERIC only
            //Whitespace is forbidden in mode/command pairs
            //  That is how the command will be delimited

            //Command commands need to be unique
            //Command names do not need to be unique:
            //   Meaning, if needed (hopefully shoudn't be common), you could have 
            //   different top-level commands for the same report/ETL command name.
            //   BUT, you should prefer to leverage the modes
            //Mode commands need to be unique for each Command
            //Mode types/name do not need to be unique:
            //   Meaning, You could have two different styles of say,  daterange modes
            //   as long as they have different mode commands for the top-level command
            //Argument names need to be unique for each unique command/mode command pair

            #endregion

            model.AddCommand("car", "CashAdvanceReport", "Cash Adavance Report")
                .AddDelegate(GetCashAdvanceReport.Go)
                .AddMode("n", Modes.Normal, "Runs for prior day")
                .AddMode("d", Modes.DateRange, "Runs for given date range")
                    .AddArgument("Begin", "Begin Date", typeof(DateTime))
                    .AddArgument("End", "End Date", typeof(DateTime));

            model.AddCommand("lpr", "LeasePaymentReport", "Lease Payment Report")
                .AddDelegate(GetLeasePaymentReport.Go)
                .AddMode("n", Modes.Normal, "Runs for prior day")
                .AddMode("d", Modes.DateRange, "Runs for given date range")
                    .AddArgument("Begin", "Begin Date", typeof(DateTime))
                    .AddArgument("End", "End Date", typeof(DateTime));

            model.AddCommand("tvr", "TellerVolumeReport", "Teller Volume Report")
                .AddDelegate(GetTellerVolumeReport.Go)
                .AddMode("n", Modes.Normal, "Runs for prior Month")
                .AddMode("d", Modes.DateRange, "Runs for given Month of year YYYYmmDD -> mm of YYYY")
                    .AddArgument("Month", "Date of Month", typeof(DateTime));

            model.AddCommand("clean", "FileCleanupTool", "File Cleanup Tool")
                .AddDelegate(RunFileCleanupTool.Go)
                .AddMode("n", Modes.Normal, "Deletes all older than 90 days")
                .AddMode("n30", Modes.Past30Days, "Deletes all older than 30 days")
                .AddMode("n7", Modes.Past7Days, "Deletes all older than 7 days")
                .AddMode("c", Modes.DateRange, "Deletes all files older then date")     //c for custom (may change)
                    .AddArgument("Older", "CleanAllOlderThen", typeof(DateTime));

            model.AddCommand("ctrExemptionUpdater", "CtrFlagExemptionUpdate", "Update CTR Flag in AML_ACCOUNTS table")
                .AddDelegate(SetAmlAccounts.Go)
                .AddMode("n", Modes.Normal, "Updates AML Accounts for today")
                .AddMode("d", Modes.DateRange, "Reads file after specified creation date")
                    .AddArgument("AfterDate", "Look for files after this date", typeof(DateTime));

            model.AddCommand("bpuejfmt", "EjExtractUtility", "Runs the EJ Extract utility on the previous day")
                .AddDelegate(EjExtractUtil.Go)
                .AddMode("n", Modes.Normal, "Run Ej Extract for yesterday");

            model.AddCommand("ETLJobMonitor", "RunETLJobMonitor", "Emails the Developers a report of all job types that ran yesterday")
                .AddDelegate(RunETLJobMonitor.Go)
                .AddMode("n", Modes.Normal, "Emails out the Developers");

            model.AddCommand("ldo", "LargeDollarOverridesReport", "Generates report on large check deposits which require a supervisor override")
                .AddDelegate(GetLrgDollarOverridesReport.Go)
                .AddMode("n", Modes.Normal, "Generates report with minimum check deposit amount set to $25,000");

            model.AddCommand("tdr", "TellerDepositReport", "Adhoc report to determine all deposits with transit checks over $5,000 in total to evaluate " +
                                                           "the final large dollar limits that will prompt for consumer and business Reg CC holds")
                .AddDelegate(GetTransitDepositReport.Go)
                .AddMode("n", Modes.Normal, "Generates report for past 3 months")
                .AddMode("m", Modes.PastMonth, "Generates report for past 1 month")
                .AddMode("n7", Modes.Past7Days, "Generates report for past 7 days");

            model.AddCommand("ocr", "OfficalCheckReport", "Runs Offical Chack Reports for Deposit Operations Report")
                .AddDelegate(GetOfficialCheckReport.Go)
                .AddMode("n", Modes.Normal, "Runs OCM for previous day")
                .AddMode("d", Modes.DateRange, "Runs for given date range")
                        .AddArgument("Begin", "Begin Date", typeof(DateTime))
                        .AddArgument("End", "End Date", typeof(DateTime));

            model.AddCommand("mil", "MonetaryInstrumentLogReport", "Generates the Monetary Instrument Log Report")
                .AddDelegate(GetMonetaryInstrumentLogReport.Go)
                .AddMode("n", Modes.Normal, "Runs Monetary Instrument Log Report");

            model.AddCommand("mim", "MIMonthlyMonitoringReport", "Generates the MI Monitoring Report")
                .AddDelegate(GetMIMonthlyMonitoring.Go)
                .AddMode("n", Modes.Normal, "Runs MI Monitoring Report (past 90 days)");

            model.AddCommand("normalChain", "NormalChain", "Chain Cash Advance, Lease Payments, AML Updater, And AutoCleanup in Normal Mode")
                .AddChain("bpuejfmt", "n")
                .AddChain("car", "n")
                .AddChain("lpr", "n")                   
                .AddChain("ocr", "n")               // take out for deployment if not ready
                .AddChain("clean", "n30");

            model.AddCommand("weeklyChain", "WeeklyChain", "Chain Large Dollar Overrides in Normal Mode")
                .AddChain("ldo", "n");

            model.AddCommand("monthlyChain", "MonthlyChain", "Chain TellerVolumeReport in Normal Mode")
                .AddChain("tvr", "n")
                .AddChain("mim", "n")
                .AddChain("tdr", "n");

            model.AddCommand("testChain", "TestChain", "Chain Reports as a 1 off Run in Normal Mode")
                .AddChain("tvr", "n")
                .AddChain("tdr", "n");
        }
    }
}