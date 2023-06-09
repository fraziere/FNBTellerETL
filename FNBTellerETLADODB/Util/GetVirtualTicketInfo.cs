using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.Util
{
    public static class GetVirtualTicketInfo
    {
        private static readonly string sp_getVitualTicketInfo = "etl.getVirtualTicketConfig";

        public static List<VirtualTicketModel> GetVirtualTicketDescription(string routingNumber, string accountNumber, string tranCode, string serial, string itemType)
        {
            var retVal = new List<VirtualTicketModel>();

            var ds = GetDataSet(routingNumber, accountNumber, tranCode, serial, itemType);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new VirtualTicketModel();
                    holder.regionID = dr.Field<string>("Region");
                    holder.officeID = dr.Field<string>("Office");
                    holder.itemName = dr.Field<string>("ItemName");
                    holder.description = dr.Field<string>("Description");
                    holder.routingNumber = dr.Field<string>("RoutingNumber");
                    holder.accountNumber = dr.Field<string>("AccountNumber");
                    holder.tranCode = dr.Field<string>("TranCode");
                    holder.serial = dr.Field<string>("Serial");
                    holder.itemType = dr.Field<string>("ItemType");

                    retVal.Add(holder);
                }
            }

            return retVal;
        }

        private static DataSet GetDataSet(string routingNumber, string accountNumber, string tranCode, string serial, string itemType)
        {
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlRoutingNum = new SqlParameter("@RoutingNumber", SqlDbType.VarChar);
                sqlRoutingNum.Value = routingNumber;
                parms.Add(sqlRoutingNum);
                var sqlAccountNumber = new SqlParameter("@AccountNumber", SqlDbType.VarChar);
                sqlAccountNumber.Value = accountNumber;
                parms.Add(sqlAccountNumber);
                var sqlTranCode = new SqlParameter("@TranCode", SqlDbType.VarChar);
                sqlTranCode.Value = tranCode;
                parms.Add(sqlTranCode);
                var sqlSerial = new SqlParameter("@Serial", SqlDbType.VarChar);
                sqlSerial.Value = serial;
                parms.Add(sqlSerial);
                var sqlItemType = new SqlParameter("@ItemType", SqlDbType.VarChar);
                sqlItemType.Value = itemType;
                parms.Add(sqlItemType);
                return ExecuteDataset(Database.FNBCustom, sp_getVitualTicketInfo, parms);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class VirtualTicketModel
    {
        public string regionID { get; set; }
        public string officeID { get; set; }
        public string itemName { get; set; }
        public string description { get; set; }
        public string routingNumber { get; set; }
        public string accountNumber { get; set; }
        public string tranCode { get; set; }
        public string serial { get; set; }
        public string itemType { get; set; }
    }
}