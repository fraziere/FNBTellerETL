using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FNBTellerETLADODB.ADODB;

namespace FNBTellerETLADODB.OfficialCheckReport
{
    public static class GetOfficalCheckInfo
    {
        private static readonly string sp_getOfficalCheck = "etl.GetOfficalCheckTran";

        public static List<OfficalCheckModel> Get(DateTime from, DateTime to)
        {
            var retVal = new List<OfficalCheckModel>();

            var ds = GetDataSet(from, to);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var holder = new OfficalCheckModel();
                    holder.regionID = dr.Field<string>("region_id");
                    holder.branchNumber = dr.Field<string>("office_id");
                    holder.cashboxNumber = dr.Field<string>("cashsum_id");

                    holder.operId = (dr.Field<string>("oper_id") ?? "");
                    holder.employeeName = (dr.Field<string>("EmployeeName") ?? "").Replace(',', ' ').Trim();
                    holder.sysLimitsProf = (dr.Field<string>("LimitsProfDesc") ?? "").Replace(',',' ');
                    holder.sysOperSecProf = (dr.Field<string>("OperSecProfDesc") ?? "").Replace(',', ' ');
                    holder.overrideTypeFlag = (dr.Field<string>("OVERRIDETYPE") ?? "");
                    holder.overrideId = (dr.Field<string>("OVERRIDEID") ?? "");

                    holder.overriderName = (dr.Field<string>("OverriderName") ?? "").Replace(',', ' ').Trim(); 
                    holder.overriderLimits = (dr.Field<string>("OverriderLimits") ?? "").Replace(',', ' ').Trim();



                    holder.itemnumber = dr.Field<int>("itemnumber");
                    holder.serial = (dr.Field<string>("serial") ?? "");
                    holder.field6 = (dr.Field<string>("field6") ?? "");
                    holder.abanumber = (dr.Field<string>("abanumber") ?? "");
                    holder.field4 = (dr.Field<string>("field4") ?? "");
                    holder.account = (dr.Field<string>("account") ?? "");
                    holder.trancode = (dr.Field<string>("trancode") ?? "");

                    holder.ammount = dr.Field<decimal>("amount");

                    holder.officeName = (dr.Field<string>("OfficeName") ?? "").Replace(',', ' ');

                    holder.procDate = dr.Field<DateTime>("procdate");

                    holder.transeq = dr.Field<int>("transeq");
                    holder.sequence = dr.Field<int>("sequence");
                    holder.ISN = dr.Field<string>("ISN");

                    retVal.Add(holder);
                }
            }

            return retVal;
        }

        private static DataSet GetDataSet(DateTime from, DateTime to)
        {
            try
            {
                List<SqlParameter> parms = new List<SqlParameter>();
                var sqlFrom = new SqlParameter("@ProcDateLow", SqlDbType.DateTime);
                sqlFrom.Value = from;
                parms.Add(sqlFrom);
                var sqlTo = new SqlParameter("@ProcDateHigh", SqlDbType.DateTime);
                sqlTo.Value = to;
                parms.Add(sqlTo);
                return ExecuteDataset(Database.FNBCustom, sp_getOfficalCheck, parms);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class OfficalCheckModel
    {
        public string regionID { get; set; }
        public string branchNumber { get; set; }
        public string cashboxNumber { get; set; }

        public string operId { get; set; }
        public string employeeName { get; set; }
        public string sysLimitsProf { get; set; }
        public string sysOperSecProf { get; set; }
        public string overrideTypeFlag { get; set; }
        public string overrideId { get; set; }
        public string overriderName { get; set; }
        public string overriderLimits { get; set; }
        public int itemnumber { get; set; }
        public string serial { get; set; }
        public string field6 { get; set; }
        public string abanumber { get; set; }
        public string field4 { get; set; }
        public string account { get; set; }
        public string trancode { get; set; }

        public decimal ammount { get; set; }

        public string officeName { get; set; }

        public DateTime procDate { get; set; }

        public int transeq { get; set; }
        public int sequence { get; set; }
        public string ISN { get; set; }
    }
}
