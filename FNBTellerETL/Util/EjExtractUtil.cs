using FNBCoreETL.Framework;
using System;
using System.Diagnostics;

namespace FNBTellerETL.Util
{
    public static class EjExtractUtil
    {
        /// <summary>
        /// Executes BPUEJFMT.exe (EJ Formatter) for the previous day.
        /// </summary>
        public static ETLJobOut Go(ETLJobIn job)
        {
            switch(job.ModeName)
            {
                case "Normal":
                    var yesterday = DateTime.Now.AddDays(-1);
                    RunEjExtractTool(yesterday);
                    break;
                default:
                    throw new ApplicationException($"job.ModeName is not recognized: ${job.ModeName}");
            }

            return new ETLJobOut(0, "EJ Extract Tool Completed Successfully");
        }

        public static void RunEjExtractTool(DateTime atThisDate)
        {
            string exeFilePath = Config.FileConfiguration.EjExtractUtil.BPUEJFMTexeFullPath.Value;
            string workingDirectory = Config.FileConfiguration.EjExtractUtil.OutputXMLDir.Value;
            int exeTimeoutInMS = Config.FileConfiguration.EjExtractUtil.BPUEJFMTexeTimeoutMS.Value;

            string procDate = atThisDate.ToString("yyyyMMdd");

            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.WorkingDirectory = workingDirectory;
                myProcess.StartInfo.FileName = exeFilePath;
                myProcess.StartInfo.Arguments = $"ARGOENT {procDate} {procDate}";

                var sw = Stopwatch.StartNew();
                myProcess.Start();
                myProcess.WaitForExit(exeTimeoutInMS);

                if (sw.ElapsedMilliseconds > exeTimeoutInMS)
                {
                    throw new TimeoutException("BPUEJFMT Blob expander timed out\n ActualTimeMS: " + sw.ElapsedMilliseconds + "\n TimeoutLimitMS: " + exeTimeoutInMS);
                }
            }
        }
    }
}
