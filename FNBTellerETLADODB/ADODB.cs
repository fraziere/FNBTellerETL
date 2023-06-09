using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETLADODB
{
    public static class ADODB
    {

        private static string _FNBCustomConnStr;

        /// <summary>
        /// Asssign different databases needed and set their connection strings inside
        /// GetConnectionString() below
        /// </summary>
        internal enum Database
        {
            FNBCustom
        }


        /// <summary>
        /// Retrieves the connection string based on database type.  If you need to add
        /// A Connection string, please do so in the DataAccessBase class.  If audit and application
        /// are the same, you can simply have it fall through the case
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        internal static string GetConnectionString(Database dbType)
        {
            switch (dbType)
            {
                //case Database.ArgoEnt:
                    //return Configuration.Database.ConnStrArgoEnt.Value;
                case Database.FNBCustom:
                    {
                        if (String.IsNullOrEmpty(Bootstrap.FNBCustomConnStr))
                            throw new ApplicationException("Failed to Set Connection String for FNBCustom.  Failing...");

                        return Bootstrap.FNBCustomConnStr;
                    }
                default:
                    throw new ArgumentException("Invalid database type");

            }
        }
        #region ADO.NET

        /// <summary>
        /// This method will execute a SQL stored procedure and return the results in a DataSet.  The
        /// string parameters will be passed through a SQL XSS replace function.  
        /// The connection is opened and closed by this method.
        /// 
        /// Opens/Closes the connetion
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="spName"></param>
        /// <param name="parameters">Optional</param>
        /// <param name="timeOutInSeconds">Optional. Default is 30 seconds</param>
        /// <returns></returns>
        internal static DataSet ExecuteDataset(Database databaseType, string spName, List<SqlParameter> sParams = null, int? timeOutInSeconds = null)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(GetConnectionString(databaseType)))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (timeOutInSeconds.HasValue)
                        cmd.CommandTimeout = timeOutInSeconds.Value;

                    if (sParams != null)
                    {
                        foreach (SqlParameter sParam in sParams)
                            cmd.Parameters.Add(sParam);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }

            }

            return ds;
        }

        /// <summary>
        /// This method does the exact same as the other ExecuteDataSet, but should ONLY BE USED FOR
        /// TRANSACTIONS.  Your sqlConnetion should be created and opened before calling.
        /// 
        /// IMPORTANT! You must close the connection.  This method will leave it open.
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="spName"></param>
        /// <param name="parameters">Optional</param>
        /// <param name="timeOutInSeconds">Optional. Default is 30 seconds</param>
        /// <returns></returns>
        internal static DataSet ExecuteDataset(SqlConnection sqlConn, string spName, SqlTransaction sqlTran, List<SqlParameter> sParams = null, int? timeOutInSeconds = null)
        {
            DataSet ds = new DataSet();

            using (SqlCommand cmd = new SqlCommand(spName, sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (timeOutInSeconds.HasValue)
                    cmd.CommandTimeout = timeOutInSeconds.Value;

                if (sqlTran != null)
                    cmd.Transaction = sqlTran;

                if (sParams != null)
                {
                    foreach (SqlParameter sParam in sParams)
                        cmd.Parameters.Add(sParam);
                }

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }
            }

            return ds;
        }

        /// <summary>
        /// This method will execute a sql statement and return the results in a DataSet.
        /// This method expects a sql command, and it's parameters (if any), to be passed in as arguments.
        /// <para>The connection is opened and closed by this method.</para>
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="sqlCommand">Sql Command to Execute. If params are used, pass them in as well</param>
        /// <param name="sParams">Optional</param>
        /// <param name="timeOutInSeconds">Optional. Default is 30 seconds</param>
        /// <returns></returns>
        internal static DataSet ExecuteQuery(Database databaseType, string sqlCommand, List<SqlParameter> sParams = null, int? timeOutInSeconds = null)
        {
            DataSet ds = new DataSet();

            using (var conn = new SqlConnection(GetConnectionString(databaseType)))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sqlCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    if (timeOutInSeconds.HasValue)
                        cmd.CommandTimeout = timeOutInSeconds.Value;

                    if (sParams != null)
                    {
                        foreach (SqlParameter sParam in sParams)
                            cmd.Parameters.Add(sParam);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }

            return ds;
        }

        /// <summary>
        /// This method will execute a SQL stored procedure via and return the rows affected.
        /// IMPORTANT!!! From MSDN.  Rows Affected is defined as:
        /// 
        /// For UPDATE, INSERT, and DELETE statements, the return value is the number of
        /// rows affected by the command. When a trigger exists on a table being inserted
        /// or updated, the return value includes the number of rows affected by both the
        /// insert or update operation and the number of rows affected by the trigger
        /// or triggers. For all other types of statements, the return value is -1.
        /// If a rollback occurs, the return value is also -1.
        /// 
        /// Opens/Closes the connetion
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="spName"></param>
        /// <param name="sParams">Optional</param>
        /// <param name="timeOutInSeconds">Optional. Default is 30 seconds</param>
        /// <returns>Be careful using this value for validation. See method explanation</returns>
        internal static int ExecuteNonQuery(Database databaseType, string spName, List<SqlParameter> sParams = null, int? timeOutInSeconds = null)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString(databaseType)))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (timeOutInSeconds.HasValue)
                        cmd.CommandTimeout = timeOutInSeconds.Value;

                    if (sParams != null)
                    {
                        foreach (SqlParameter sParam in sParams)
                            cmd.Parameters.Add(sParam);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }

        }

        /// <summary>
        /// This method does the exact same as the other ExecuteNonQuery, but should ONLY BE USED FOR
        /// TRANSACTIONS.  Your sqlConnetion should be created and opened before calling.
        /// 
        /// IMPORTANT! You must close the connection.  This method will leave it open.
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="spName"></param>
        /// <param name="parameters">Optional</param>
        /// <param name="timeOutInSeconds">Optional. Default is 30 seconds</param>
        /// <returns></returns>
        internal static int ExecuteNonQuery(SqlConnection sqlConn, string spName, SqlTransaction sqlTran, List<SqlParameter> sParams = null, int? timeOutInSeconds = null)
        {

            using (SqlCommand cmd = new SqlCommand(spName, sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (timeOutInSeconds.HasValue)
                    cmd.CommandTimeout = timeOutInSeconds.Value;

                if (sqlTran != null)
                    cmd.Transaction = sqlTran;

                if (sParams != null)
                {
                    foreach (SqlParameter sParam in sParams)
                        cmd.Parameters.Add(sParam);
                }

                return cmd.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// This method will execute a SQL stored procedure via and  
        /// returns the first column of the first row in the result set
        /// returned by the query. Additional columns or rows are ignored.
        /// 
        /// Opens/Closes the connetion
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="spName"></param>
        /// <param name="sParams">Optional</param>
        /// <param name="timeOutInSeconds">Optional.  Default is 30 seconds</param>
        /// <returns></returns>
        internal static object ExecuteScalar(string connStr, string spName, List<SqlParameter> sParams = null, int? timeOutInSeconds = null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (timeOutInSeconds.HasValue)
                        cmd.CommandTimeout = timeOutInSeconds.Value;

                    if (sParams != null)
                    {
                        foreach (SqlParameter sParam in sParams)
                            cmd.Parameters.Add(sParam);
                    }

                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// This method does the exact same as the other ExecuteScalar, but should ONLY BE USED FOR
        /// TRANSACTIONS.  Your sqlConnetion should be created and opened before calling.
        /// 
        /// IMPORTANT! You must close the connection.  This method will leave it open.
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="spName"></param>
        /// <param name="parameters">Optional</param>
        /// <param name="timeOutInSeconds">Optional. Default is 30 seconds</param>
        /// <returns></returns>
        internal static object ExecuteScalar(SqlConnection sqlConn, string spName, SqlTransaction sqlTran, List<SqlParameter> sParams = null, int? timeOutInSeconds = null)
        {
            using (SqlCommand cmd = new SqlCommand(spName, sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (timeOutInSeconds.HasValue)
                    cmd.CommandTimeout = timeOutInSeconds.Value;

                if (sqlTran != null)
                    cmd.Transaction = sqlTran;

                if (sParams != null)
                {
                    foreach (SqlParameter sParam in sParams)
                        cmd.Parameters.Add(sParam);
                }

                return cmd.ExecuteScalar();
            }
        }

        #endregion
    }
}
