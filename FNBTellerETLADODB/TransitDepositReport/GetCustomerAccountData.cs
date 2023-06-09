using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.TransitDepositReport
{
    public static class GetCustomerAccountData
    {
        private static readonly string sp_getCustomerAccountData = "etl.GetCustomerAccountData";
        //private static readonly string sp_getCustomerAccountData = "etl.TEST_DELETEME_AUG01_GetCustomerAccountData";        

        public static List<CustomerAccountModel> Get(List<string> accountShortList)
        {
            var retVal = new List<CustomerAccountModel>();


            using (var ds = GetDataSet(accountShortList))
            {
                if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var holder = new CustomerAccountModel();

                        holder.transit = dr.Field<string>("transit");
                        holder.acct = dr.Field<string>("acct");
                        holder.description = dr.Field<string>("description");
                        holder.customerId = dr.Field<string>("customer_id");
                        holder.fullName = dr.Field<string>("full_name");
                        holder.entityType = dr.Field<string>("entity_type");

                        retVal.Add(holder);
                    }
                }
            }

            return FilterAccountList(accountShortList, retVal);

            //return retVal;
        }

        private static List<CustomerAccountModel> FilterAccountList(List<string> accountShortList, List<CustomerAccountModel> inflowList)
        {
            var retVal = new List<CustomerAccountModel>();


            foreach(var accountNum in accountShortList)
            {
                var accountXShortList = inflowList.Where(x => (x.acct == accountNum));

                if(accountXShortList.Where(x => x.entityType == "F").Count() > 0)
                {
                    retVal.Add(accountXShortList.Where(x => x.entityType == "F").First());
                }
                else if (accountXShortList.Where(x => x.entityType == "B").Count() > 0)
                {
                    retVal.Add(accountXShortList.Where(x => x.entityType == "B").First());
                }
                else if (accountXShortList.Where(x => x.entityType == "I").Count() > 0)
                {
                    retVal.Add(accountXShortList.Where(x => x.entityType == "I").First());
                }
                else if (accountXShortList.Where(x => x.entityType == "U").Count() > 0)
                {
                    retVal.Add(accountXShortList.Where(x => x.entityType == "U").First());
                }
                else
                {
                    //this is if the account number makes no sense
                }
            }

            return retVal;
        }

        private static DataSet GetDataSet(List<string> accountShortList)
        {

            DataTable dt = new DataTable();
            DataColumn COLUMN = new DataColumn();
            COLUMN.ColumnName = "accountNum";
            COLUMN.DataType = typeof(string);
            dt.Columns.Add(COLUMN);

            foreach(var item in accountShortList)
            {
                DataRow DR = dt.NewRow();
                DR[0] = item;
                dt.Rows.Add(DR);
            }



            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                
                var sqlFrom = new SqlParameter("@AccountList", SqlDbType.Structured);
                sqlFrom.Value = dt;
                parms.Add(sqlFrom);

                //SqlParameter tvpParam = insertCommand.Parameters.AddWithValue("@tvpNewCategories", addedCategories);

                /*
                var sqlTo = new SqlParameter("@ProcDateHigh", SqlDbType.DateTime);
                sqlTo.Value = to;
                parms.Add(sqlTo);
                */
                return ExecuteDataset(Database.FNBCustom, sp_getCustomerAccountData, parms, timeOutInSeconds:900);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public struct CustomerAccountModel
    {
        public string transit { get; set; }
        public string acct { get; set; }
        public string description { get; set; }
        public string customerId { get; set; }
        public string fullName { get; set; }
        public string entityType { get; set; }
        //public int contypeId { get; set; }


    }
}
