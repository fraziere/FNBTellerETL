using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.AmlAccountsUpdater
{
    public static class SetAmlAccounts
    {
        private static readonly string sp_setAmlAccounts = "etl.SetAmlAccountsCtrFlag";
        private static readonly string sp_setAllAmlAccounts = "etl.SetAllAmlAccountsCtrFlag";

        /// <summary>
        /// Stored Proc sets all ctr_exemption flags to 'N'
        /// </summary>
        public static void SetAll()
        {
            //Set timeout to 120 seconds
            ExecuteNonQuery(Database.FNBCustom, sp_setAllAmlAccounts, null, 120);
        }

        /// <summary>
        /// Iterates accountsToBeUpdated and calls a stored proc to update the ctr_exemption to 'Y'
        /// Note: ctr_exemption flag values are only 'Y' or 'N'
        /// </summary>
        /// <param name="accountsToBeUpdated"></param>
        public static List<string> Set(List<string> accountsToBeUpdated)
        {
            List<string> accountsUpdated = new List<string>();

            foreach(var account in accountsToBeUpdated)
            {
                
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlAcctType = new SqlParameter("@AcctType", SqlDbType.VarChar);
                var sqlAcctNum = new SqlParameter("@AcctNum", SqlDbType.VarChar);
                sqlAcctType.Value = account.Substring(0,3);
                sqlAcctNum.Value = account.Substring(4, account.Length);
                parms.Add(sqlAcctType);
                parms.Add(sqlAcctNum);

                //Set timeout to 300 seconds
                if (ExecuteNonQuery(Database.FNBCustom, sp_setAmlAccounts, parms, 300) >= 1)
                {
                    accountsUpdated.Add(account);
                }
            }
            return accountsUpdated;
        }

    }
}
