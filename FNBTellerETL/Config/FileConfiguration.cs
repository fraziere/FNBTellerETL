using FNBTellerETL.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL.Config
{
    public static class FileConfiguration
    {
        public static FileConfigItem.AsString Environment
        {
            get
            {
                return new FileConfigItem.AsString("Environment");
            }
        }

        public static class Database
        {
            //public static ConfigItem.AsString ConnStrArgoEnt
            //{
            //    get
            //    {
            //        return new ConfigItem.AsString("Database.ConnStrArgoEnt");
            //    }
            //}

            public static FileConfigItem.AsString ConnStrFNBCustom
            {
                get
                {
                    return new FileConfigItem.AsString("Database.ConnStrFNBCustom");
                }
            }
        }

        public static class Email
        {
            public static FileConfigItem.AsString SmtpAddress
            {
                get
                {
                    return new FileConfigItem.AsString("Email.SmtpAddress");
                }
            }

            public static FileConfigItem.AsString SendProgramReportsFrom
            {
                get
                {
                    return new FileConfigItem.AsString("Email.SendReportsFrom");
                }
            }

            public static FileConfigItem.AsBool SendProgramEmailAlert
            {
                get
                {
                    return new FileConfigItem.AsBool("Email.SendProgramEmailAlert", false, false);
                }
            }

            public static FileConfigItem.AsString SendEmailAlertFrom
            {
                get
                {
                    return new FileConfigItem.AsString("Email.SendProgramEmailAlertFrom");
                }
            }

            public static List<string> SendProgramEmailAlertTo
            {
                get
                {
                    try
                    {
                        string holder = SendProgramEmailAlertToValue.Value;
                        return StrUtil.SemiColonDelimited(holder);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }
            private static FileConfigItem.AsString SendProgramEmailAlertToValue
            {
                get
                {
                    return new FileConfigItem.AsString("Email.SendProgramEmailAlertTo");
                }
            }
        }

        public static class CashAdvanceReports
        {
            public static FileConfigItem.AsString ReportFileLocation
            {
                get
                {
                    return new FileConfigItem.AsString("CashAdvanceReport.ReportFileLocation");
                }
            }

            public static FileConfigItem.AsString SendReportsTo
            {
                get
                {
                    return new FileConfigItem.AsString("CashAdvanceReport.SendReportsTo");
                }
            }
        }

        public static class LeasePaymentReport
        {
            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        string holder = SendReportsToValue.Value;
                        return StrUtil.SemiColonDelimited(holder);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }
            private static FileConfigItem.AsString SendReportsToValue
            {
                get
                {
                    return new FileConfigItem.AsString("LeasePaymentsReport.SendReportsTo");
                }
            }

            public static FileConfigItem.AsString OutputCSVDir
            {
                get
                {
                    return new FileConfigItem.AsString("LeasePaymentsReport.OutputCSVDir");
                }
            }

            public static FileConfigItem.AsString LCLHardcodedXMLLoc
            {
                get
                {
                    return new FileConfigItem.AsString("LeasePaymentsReport.LCLHardcodedXMLLoc");
                }
            }
        }

        public static class TellerVolumeReport
        {
            public static FileConfigItem.AsString ReportFileLocation
            {
                get
                {
                    return new FileConfigItem.AsString("TellerVolumeReport.ReportFileLocation");
                }
            }
            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        string holder = SendReportsToValue.Value;
                        return StrUtil.SemiColonDelimited(holder);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }
            private static FileConfigItem.AsString SendReportsToValue
            {
                get
                {
                    return new FileConfigItem.AsString("TellerVolumeReport.SendReportsTo");
                }
            }
        }

        public static class FileCleanupTool
        {
            public static List<string> FileCleanupDirs
            {
                get
                {
                    try
                    {
                        string holder = FileCleanupDirToValue.Value;
                        return StrUtil.SemiColonDelimited(holder);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid FileCleanupDirToValue", ex);
                    }
                }
            }

            private static FileConfigItem.AsString FileCleanupDirToValue
            {
                get
                {
                    return new FileConfigItem.AsString("FileCleanupTool.FoldersToClean");
                }
            }


        }

        public static class AmlAccountUpdater
        {
            public static FileConfigItem.AsString SendJobAlertsTo
            {
                get
                {
                    return new FileConfigItem.AsString("AmlAccountsUpdater.SendJobAlertsTo");
                }
            }

            public static FileConfigItem.AsString InputFileDir
            {
                get
                {
                    return new FileConfigItem.AsString("AmlAccountsUpdater.InputFileDir");
                }
            }

            public static FileConfigItem.AsString HistoryDir
            {
                get
                {
                    return new FileConfigItem.AsString("AmlAccountsUpdater.HistoryDir");
                }
            }
        }

        public static class EjExtractUtil
        {
            public static FileConfigItem.AsString BPUEJFMTexeFullPath
            {
                get
                {
                    return new FileConfigItem.AsString("EjExtractUtil.BPUEJFMTexeFullPath");
                }
            }

            public static FileConfigItem.AsInt BPUEJFMTexeTimeoutMS
            {
                get
                {
                    return new FileConfigItem.AsInt("EjExtractUtil.BPUEJFMTexeTimeoutMS");
                }
            }

            public static FileConfigItem.AsString OutputXMLDir
            {
                get
                {
                    return new FileConfigItem.AsString("EjExtractUtil.OutputXMLDir");
                }
            }
        }

        public static class ETLJobMonitor
        {
            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        string holder = SendReportsToValue.Value;
                        return StrUtil.SemiColonDelimited(holder);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }
            private static FileConfigItem.AsString SendReportsToValue
            {
                get
                {
                    return new FileConfigItem.AsString("ETLJobMonitor.SendEmailAlertsTo");
                }
            }
        }

        public static class LargeDollarOverridesReport
        {
            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        return StrUtil.SemiColonDelimited(SendReportsToValue.Value);
                    }
                    catch (Exception e)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", e);
                    }
                }
            }

            public static FileConfigItem.AsString ReportFileLocation
            {
                get
                {
                    return new FileConfigItem.AsString("LrgDollarOvrrideRep.ReportFileLocation");
                }
            }

            private static FileConfigItem.AsString SendReportsToValue
            {
                get
                {
                    return new FileConfigItem.AsString("LrgDollarOvrrideRep.SendEmailAlertsTo");
                }
            }
        }

        public static class OfficalCheckReport
        {
            public static FileConfigItem.AsString ReportFileLocation
            {
                get
                {
                    return new FileConfigItem.AsString("OfficalCheckReport.ReportFileLocation");
                }
            }
            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        string holder = SendReportsToValue.Value;
                        return StrUtil.SemiColonDelimited(holder);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }
            private static FileConfigItem.AsString SendReportsToValue
            {
                get
                {
                    return new FileConfigItem.AsString("OfficalCheckReport.SendReportsTo");
                }
            }
        }

        public static class TransitDepositReport
        {
            public static FileConfigItem.AsString ReportFileLocation
            {
                get
                {
                    return new FileConfigItem.AsString("TransitDepositReport.ReportFileLocation");
                }
            }
            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        string holder = SendReportsToValue.Value;
                        return StrUtil.SemiColonDelimited(holder);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }
            private static FileConfigItem.AsString SendReportsToValue
            {
                get
                {
                    return new FileConfigItem.AsString("TransitDepositReport.SendReportsTo");
                }
            }
        }

        public static class MonetaryInstrumentLogReport
        {
            public static FileConfigItem.AsString ReportFileLocation
            {
                get { return new FileConfigItem.AsString("MonetaryInstrumentLogReport.ReportFileLocation"); }
            }

            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        return StrUtil.SemiColonDelimited(SendReportsToValue.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }

            private static FileConfigItem.AsString SendReportsToValue
            {
                get { return new FileConfigItem.AsString("MonetaryInstrumentLogReport.SendReportsTo"); }
            }
        }

        public static class MIMonthlyMonitoringReport
        {
            public static FileConfigItem.AsString ReportFileLocation
            {
                get { return new FileConfigItem.AsString("MIMonthlyMonitoringReport.ReportFileLocation"); }
            }

            public static List<string> SendReportsTo
            {
                get
                {
                    try
                    {
                        return StrUtil.SemiColonDelimited(SendReportsToValue.Value);
                    }
                    catch (Exception ex)
                    {
                        throw new FileConfigurationException("Invalid SendProgramEmailAlertTo", ex);
                    }
                }
            }

            private static FileConfigItem.AsString SendReportsToValue
            {
                get { return new FileConfigItem.AsString("MIMonthlyMonitoringReport.SendReportsTo"); }
            }
        }
    }
}
