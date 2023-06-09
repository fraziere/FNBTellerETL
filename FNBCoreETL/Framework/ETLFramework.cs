using FNBCoreETL.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNBCoreETL.Logging;

namespace FNBCoreETL.Framework
{
    public abstract class ETLFramework
    {
        
        public abstract void BuildModel(ICommand model);

        /// <summary>
        /// Go for console app
        /// ***IMPORTANT DO NOT WRAP IN A TRY CATCH. Let errors bubble up to the framework
        /// </summary>
        public void Go(string[] args)
        {
            var appSw = new Stopwatch();
            appSw.Start();

            try
            {
                var sb = new StringBuilder();
                if (args != null && args.Length > 0)
                {
                    foreach (var arg in args)
                    {
                        sb.Append(arg + " ");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }

                Logger.AppLog.LogInfo(Logger.AppLog.State.Start, "ConsoleApp: CmdLine", sb.ToString(), null);
            }
            catch (Exception ex)
            {
                Logger.AppLog.LogError(Logger.AppLog.State.Start, ex, null);
            }

            IReadOnlyList<ETLJobIn> jobs = null;
            int recordCount = 0;
            try
            {
                BuildModel(ETLModel.Model);
                jobs = GetJobs(args);          
                //AppLog.LogInfo(AppLog.State.Start, "Job", "", jobs[0]);
                recordCount = RunJobs(jobs);
            }
            catch (Exception ex)
            {
                Logger.AppLog.LogError(Logger.AppLog.State.Start, ex, null);
            }
            finally
            {
                appSw.Stop();
                if (jobs != null) //if jobs is null, there damn well should have been an error
                {
                    Logger.AppLog.LogInfo(Logger.AppLog.State.End, recordCount, appSw.Elapsed.TotalSeconds, "Stat", "", jobs[0]);
                }                
            }
        }

        private int RunJobs(IReadOnlyList<ETLJobIn> jobs)
        {
            int totalRecordCount = 0;
            foreach (var job in jobs)
            {
                try
                {
                    var jobSw = new Stopwatch();
                    jobSw.Start();
                    var etlInfoOut = RunJob(job);
                    totalRecordCount += etlInfoOut.RecordCount;
                    jobSw.Stop();
                    Logger.JobLog.LogInfo(etlInfoOut.RecordCount, jobSw.Elapsed.TotalMilliseconds, job, etlInfoOut.OutputToLog);
                }
                catch (Exception ex)
                {
                    Logger.JobLog.LogError(job, ex);
                    if (job.ContinueOnError == false)
                    {
                       throw;
                    }
                }
            }

            return totalRecordCount;
        }

        private ETLJobOut RunJob(ETLJobIn job)
        {
            return job.Method(job);
        }

        private IReadOnlyList<ETLJobIn> GetJobs(string[] args)
        {
            var cmdLineParse = GetCommandLineArgs(args);
            return ETLModel.Search.GetJobs(cmdLineParse.Item1, cmdLineParse.Item2, cmdLineParse.Item3);

        }

        private Tuple<string, string, IReadOnlyList<String>> GetCommandLineArgs(string[] args)
        {
            if (args == null)
                throw new ETLFrameworkExcpetion("No command passed to framework.  Failing...");

            //for now, if args = 1 it's a chaining command (>= 1 job)
            //if args = 2 its a command/mode single job
            //if args = 3+ they're arguments to a single job

            var holder = new List<String>();
            for (var i = 2; i < args.Length; i++)
            {
                holder.Add(args[i]);
            }
            if (args.Length == 1) //TODO FIX THIS MESS
            {
                return Tuple.Create<string, string, IReadOnlyList<String>>(args[0], "", holder.AsReadOnly());
            }
            else
            {
                return Tuple.Create<string, string, IReadOnlyList<String>>(args[0], args[1], holder.AsReadOnly());
            }
            
        }
    }
}
