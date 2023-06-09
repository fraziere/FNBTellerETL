using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL.Util
{
    internal static class StrUtil
    {
        /// <summary>
        /// returns text between two string elements in a passed in string.  If none found, returns empty
        /// </summary>
        /// <param name="myText"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        public static string GetInBetweenStrings(string myText, string startString, string endString)
        {
            string retVal = string.Empty;

            try
            {
                int startIdx = myText.IndexOf(startString);
                int startCardLength = startString.Length;
                int endIdx = myText.IndexOf(endString, startIdx + startCardLength);
                if (startIdx >= 0 && endIdx > 0)
                {
                    retVal = myText.Substring(startIdx + startCardLength, endIdx - (startIdx + startCardLength)).Trim();
                }
            }
            catch { }

            return retVal;
        }
        public static List<string> SemiColonDelimited(string input)
        {
            return input.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        //public static string Truncate(this string value, int maxLength)
        //{
        //    if (string.IsNullOrEmpty(value)) return value;
        //    return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        //}
    }
}
