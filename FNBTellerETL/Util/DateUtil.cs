using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL.Util
{
    internal static class DateUtil
    {
        /// <summary>
        /// allows input as yyyyMMdd or default parsing on DateTime
        /// also outpus tuple of from and to date in correct order
        /// </summary>
        /// 
        public static DateTime customParseDateTime(string inputString)
        {            
            if (!(inputString.Contains('/') || inputString.Contains('-') || inputString.Contains('.')) && inputString.Length == 8)
            {
                //input as yyyyMMdd
                return new DateTime(int.Parse(inputString.Substring(0, 4)), int.Parse(inputString.Substring(4, 2)), int.Parse(inputString.Substring(6, 2)));
            }
            else
            {
                return DateTime.Parse(inputString);
            }       
        }

        //format from & to in correct order
        public static (DateTime fromDateOut, DateTime toDateOut) formatFromToOrder (DateTime fromDateIn, DateTime toDateIn)
        {
            if (fromDateIn > toDateIn)
            {
                var tempDate = fromDateIn;
                fromDateIn = toDateIn;
                toDateIn = tempDate;
            }
            return (fromDateIn, toDateIn);
        }

        public static (DateTime fromDateOut, DateTime toDateOut) formatFromToOrder(string fromDateIn, string toDateIn)
        {
            DateTime fromDate = customParseDateTime(fromDateIn);
            DateTime toDate = customParseDateTime(toDateIn);

            if (fromDate > toDate)
            {
                var tempDate = fromDate;
                fromDate = toDate;
                toDate = tempDate;
            }
            return (fromDate, toDate);
        }
    }
}
