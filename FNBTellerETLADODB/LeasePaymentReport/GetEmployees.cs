using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.LeasePaymentReport
{
    public static class GetEmployees
    {
        private static readonly string sp_GetInfo = "etl.EmployeeTable";

        //should this be reset on each report???
        private static DataSet employeeSet = ExecuteDataset(Database.FNBCustom, sp_GetInfo, null);


        public static string GetEmployeeName(string RegionID, string ID)
        {
            if (employeeSet != null && employeeSet.Tables.Count == 1 && employeeSet.Tables[0].Rows.Count > 0)
            {
                var filteredRows = employeeSet.Tables[0].Select($"REGIONID = '{RegionID}' AND ID = '{ID}'");
                if (filteredRows.Length > 0)
                {
                    return filteredRows[0].ItemArray[2].ToString().Trim();
                }
            }
            return "";
        }

    }
}
