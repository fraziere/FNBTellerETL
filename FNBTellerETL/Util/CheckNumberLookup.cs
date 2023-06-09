using FNBTellerETLADODB.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNBTellerETL.Util
{
    internal static class CheckNumberLookup
    {
        public static string GetPaymentType(string itemNumber)
        {
            if (itemNumber == "1031" || itemNumber == "1091" || itemNumber == "1041" || itemNumber == "1042")
            {
                return "CHECK";
            }
            if (itemNumber == "1021")
            {
                return "CASH";
            }
            if (itemNumber == "1051")
            {
                return "Debit?";
            }
            return "";
        }

        //TEST
        public static string GetPaymentType1(string itemNumber)
        {
            //Based off of Earls Email (09/13/21) 11:07am

            if(itemNumber == "1021")
            {
                return "CASH";
            }

            //1031, 1032 will always be an OnUs Check
            if (itemNumber == "1031" || itemNumber == "1032")
            {
                return "OnUs Check";
            }

            //1091 will always be a Transit Check
            if (itemNumber == "1091")
            {
                return "Transit Check";
            }

            //Item type 1051 can be check/non-check (RT, Acct# and TC line up to multiple items)
            if (itemNumber == "1051")
            {
                //check for special types
                return "Debit? or Foreign Check?";
            }

            //Potentially not used here at FNB
            if (itemNumber == "1081")
            {
                //We don't have this in DEV
                return "???";
            }

            //1033 is the item type for a special OnUs item called a “Controlled Disbursement” check.
            //This item type will only be assigned for routing number “041202511” and does not have a particular trancode or account number configured
            if (itemNumber == "1033")
            {
                return "Special OnUs - Controlled Disbursement";
            }

            //Double Check with Stephanie
            if (itemNumber == "1041" || itemNumber == "1042")
            {
                return "Check???";
            }
            return "";
        }



        //TEST
        public static string GetVirtualTicketDiscription(string routingNumber, string accountNumber, string tranCode, string serial, string itemType)
        {
            var returnVal = GetVirtualTicketInfo.GetVirtualTicketDescription(routingNumber, accountNumber, tranCode, serial, itemType);

            if(returnVal.Count == 0)
            {
                return "";
            }
            else
            {
                return returnVal.First().description;
            }
        }
    }
}
