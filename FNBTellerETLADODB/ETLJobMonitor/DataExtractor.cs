using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.ETLJobMonitor
{
    public interface IDataExtractor
    {
        IEnumerable<JobLog> JobLogToList(string dateStr);
        IEnumerable<ReportStatus> GetScheduledJobs();
    }


    public class DataExtractor : IDataExtractor
    {
        /// <summary>
        /// Given a date parameter returns the list of jobs that ran after that date
        /// </summary>
        /// <param name="date"></param>
        /// <returns>List of JobLog entires from FNBCustom database</returns>
        public IEnumerable<JobLog> JobLogToList(string dateStr)
        {
            DataSet dataSet = GetDataSet(dateStr);

            var list = dataSet.Tables[0].AsEnumerable()
                    .Select(dr => new JobLog
                    {
                        jobLogId = (int)dr.Field<Int64>("JobLogId"),
                        isError = dr.Field<Boolean>("IsError"),
                        severity = dr.Field<String>("Severity"),
                        environment = dr.Field<String>("Environment"),
                        createDate = dr.Field<DateTime>("CreateDate"),
                        command = dr.Field<String>("Command"),
                        commandName = dr.Field<String>("CommandName"),
                        mode = dr.Field<String>("Mode"),
                        modeName = dr.Field<String>("ModeName"),
                        recordCount = (dr.Field<int?>("RecordCount") ?? -1), //get as int or -1
                        roundTripMs = dr.Field<double?>("RoundTripMs") ?? 0
                    }).ToList();
            return list;
        }

        private DataSet GetDataSet(string dateStr)
        {
            string sqlCommand = "Select [JobLogId],[IsError],[Severity],[Environment],[CreateDate],[Command],[CommandName],[Mode],[ModeName],[Arguments],[ShortMsg],[LongMsg],[RecordCount],[RoundTripMs],[Output],[ServerName],[SessionID] FROM [FNBCustom].[etl].[JobLog] WHERE [CreateDate] >= @RunDate";
            
            SqlParameter param = new SqlParameter("RunDate", SqlDbType.VarChar);
            param.Value = dateStr;
            var parms = new List<SqlParameter>();
            parms.Add(param);

            return ExecuteQuery(Database.FNBCustom, sqlCommand, parms);
        }

        public IEnumerable<ReportStatus> GetScheduledJobs()
        {
            DataSet dataSet = GetScheduledJobsDataSet();

            var list = dataSet.Tables[0].AsEnumerable()
                    .Select(dr => new ReportStatus
                    {
                        reportId = dr.Field<int>("ReportId"),
                        command = dr.Field<String>("Command"),
                        commandName = dr.Field<String>("CommandName"),
                        reportEnvironment = dr.Field<String>("ReportEnvironment"),
                        reportFrequency = dr.Field<String>("ReportFrequency"),
                        createdDate = dr.Field<DateTime>("CreatedDate"),
                    }).ToList();
            return list;
        }

        private DataSet GetScheduledJobsDataSet()
        {
            string sqlCommand = "Select [ReportId],[Command],[CommandName],[ReportEnvironment],[ReportFrequency],[CreatedDate] FROM [FNBCustom].[dbo].[ReportStatus]";

            return ExecuteQuery(Database.FNBCustom, sqlCommand);
        }
    }
}
