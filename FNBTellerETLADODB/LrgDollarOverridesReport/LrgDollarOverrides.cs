using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.LrgDollarOverridesReport
{
    public static class LrgDollarOverrides
    {
        public static readonly string sp_GetLargeDollarOverrides = "etl.GetLargeDollarOverrides";

        /// <summary>
        /// Runs stored procedure against [ARGOENT].[dbo].[TLR_EJDATADETAIL] which the rows where large dollar override is greater than
        /// the amount provided
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static List<LargeDollarOverrideModel> Get(DateTime fromDate, DateTime toDate)
        {
            var retVal = new List<LargeDollarOverrideModel>();
            var ds = GetDataSet(fromDate, toDate);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new LargeDollarOverrideModel();
                    holder.Procdate = dr.Field<DateTime>("Procdate");
                    holder.RegionId = dr.Field<string>("RegionId");
                    holder.RegionName = dr.Field<string>("RegionName");
                    holder.OfficeId = dr.Field<string>("OfficeId");
                    holder.OfficeName = dr.Field<string>("OfficeName");
                    holder.CashboxId = dr.Field<string>("CashboxNum");
                    holder.TranSeqNum = dr.Field<int>("TranSeqNum");
                    holder.MsgSeqNum = dr.Field<short>("MsgSeqNum");
                    holder.OverrideId = dr.Field<string>("OverrideId");
                    holder.OverrideName = dr.Field<string>("OverrideName");
                    holder.SourceOperId = dr.Field<string>("SourceOperId");
                    holder.TellerName = dr.Field<string>("TellerName");
                    holder.AcctNum = dr.Field<string>("AcctNum");
                    holder.AmountOne = dr.Field<double?>("AmountOne");
                    holder.AmountTwo = dr.Field<double?>("AmountTwo");

                    retVal.Add(holder);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Given a LargeDollarOverrideModel object and a corresponding EJ Extract data Node (found in XML file produced from BPUEJFMT) this method
        /// fills in the blanks for the missing properties on each LDO object (i.e. AvailableBalance, CurrentBalance, etc.) which are the properties not
        /// found in the ARGOENT database
        /// </summary>
        public static void SetMissingModelValues(LargeDollarOverrideModel modelObj, XElement el)
        {
            if (el.Name.LocalName.Equals("DEPMIC2"))
            {
                //Ledger balance before Transaction = Available Balance
                //Ledger balance after Transaction = Current Balance
                modelObj.AvailableBalance = el.Element("MonetaryRs").Element("TransDetailRs").Element("AvailableBalance1").Value;
                modelObj.CurrentBalance = el.Element("MonetaryRs").Element("TransDetailRs").Element("CurrentBalance1").Value;
                modelObj.CustomerName = el.Element("MonetaryRq").Element("Tran").Element("Customer").Element("Name").Value;
            }
            else if (el.Name.LocalName.Equals("OVRDET"))
            {
                //if the name of the supervisor who performed the override was not found in the database then use the value found in the Ej Extract data
                if (String.IsNullOrEmpty(modelObj.OverrideName))
                {
                    //Note: SUPERVISOR_NAME2, SUPERVISOR_NAME3, SUPERVISOR_NAME4, SUPERVISOR_NAME5 are all nodes as well
                    var supervisorName = GetSupervisorName(el.Element("SUPERVISOR_NAME1").Value);
                    if (supervisorName != null)
                        modelObj.OverrideName = supervisorName;
                    else
                        modelObj.OverrideName = el.Element("SUPERVISOR_NAME1").Value;
                }
            }
        }

        /// <summary>
        /// Some of the <SUPERVISOR_NAME1></SUPERVISOR_NAME1> nodes in the EJ Extract data do not contain the supervisor name. Rather, they contain the supervisor's OPERID.
        /// Query the database to see if we can get the supervisor name from the SUPERVISOR_NAME1 value
        /// </summary>
        public static string GetSupervisorName(string supervisorId)
        {
            var ds = GetDataSet(supervisorId);

            //there should only be 1 supervisor ID returned from query (dont think its possible for 1 OPERID to map to multiple employees?)
            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0].Field<string>("NAME");
            }
            else
            {
                return null;
            }
        }

        public static DataSet GetDataSet(DateTime fromDate, DateTime toDate)
        {
            var fromParam = new SqlParameter("@From", SqlDbType.DateTime);
            fromParam.Value = fromDate;
            var toParam = new SqlParameter("@To", SqlDbType.DateTime);
            toParam.Value = toDate;

            var parms = new List<SqlParameter>();
            parms.Add(fromParam);
            parms.Add(toParam);

            return ExecuteDataset(Database.FNBCustom, sp_GetLargeDollarOverrides, parms);
        }

        public static DataSet GetDataSet(string str)
        {
            string sqlCommand = "SELECT [NAME] FROM [ARGOENT].[dbo].[SYS_EMPLOYEE] WITH (NOLOCK) WHERE [ID] = @Id";

            SqlParameter param = new SqlParameter("Id", SqlDbType.VarChar);
            param.Value = str;
            var parms = new List<SqlParameter>();
            parms.Add(param);

            return ExecuteQuery(Database.FNBCustom, sqlCommand, parms);
        }
    }
}
