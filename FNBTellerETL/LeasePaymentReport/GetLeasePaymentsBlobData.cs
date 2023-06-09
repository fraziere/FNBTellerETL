using FNBTellerETL.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace FNBTellerETL.LeasePaymentReport
{
    /// <summary>
    ///  Runs Blob Expander and parses the output XML file
    ///  Parsing should break on null values
    /// 
    ///  Get Range is inclusive (just calls single version multiple times)
    ///  Blob expander Only Run outside of LCL (not configured on local)
    /// </summary>
    /// 
    class GetLeasePaymentsBlobData
    {
       
        public static List<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportFormatedModel> Get(DateTime onThisDate)
        {
            var argoBLOBLeasePayments = new List<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportARGOBlobModel>();

            foreach (var element in GetFilteredElementList(onThisDate))
            {
                var holder = new FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportARGOBlobModel();

                holder.UtilCustAccountNumber = element.Element("MonetaryRq").Element("Tran").Element("Payment").Element("UtilCustAccountNumber").Value;
                holder.UtilLeaCustomerName = element.Element("MonetaryRq").Element("Tran").Element("Payment").Element("UtilLeaCustomerName").Value;
                holder.ProcDate = getDateTimeFromString(element.Element("MonetaryRq").Element("MonHeader").Element("ProcDate").Value);
                holder.Amount = element.Element("MonetaryRq").Element("Tran").Element("Payment").Element("Ticket").Element("Amount").Value;
                holder.Region = element.Element("MonetaryRq").Element("MonHeader").Element("Region").Value;
                holder.Office = element.Element("MonetaryRq").Element("MonHeader").Element("Office").Value;
                holder.OperatorID = element.Element("MonetaryRq").Element("MonHeader").Element("OperID").Value;
                holder.TranSeq = element.Element("MonetaryRq").Element("MonHeader").Element("TranSeq").Value;
                holder.MsgSeq = element.Element("MonetaryRq").Element("MonHeader").Element("MsgSeq").Value;
                holder.NAME = FNBTellerETLADODB.LeasePaymentReport.GetEmployees.GetEmployeeName(
                    element.Element("MonetaryRq").Element("MonHeader").Element("Region").Value,
                    element.Element("MonetaryRq").Element("MonHeader").Element("OperID").Value);
                holder.Cashbox = element.Element("MonetaryRq").Element("MonHeader").Element("Cashbox").Value;
                //holder.SerialNum = "--";
                //holder.field6 = "--";
                //holder.TranCode = "--";

                argoBLOBLeasePayments.Add(holder);
            }

            //Check For Reversals in EJDataDetail
            foreach (var item in argoBLOBLeasePayments.ToList())
            {
                var result = FNBTellerETLADODB.LeasePaymentReport.GetApproved_EJDATADETAIL.RunEJDataDetailApprovedQuery(
                    item.Region, item.Office, item.TranSeq, item.MsgSeq, item.Cashbox, item.ProcDate);

                //filter out all that dont map up to approved
                if (result.Count == 0)
                {
                    argoBLOBLeasePayments.Remove(item);
                }
            }

            var tranOfIntrestList = new List<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentTransactionsOfIntrestModel>();

            foreach(var argoLPItem in argoBLOBLeasePayments)
            {
                var leasePaymentTransactionList = FNBTellerETLADODB.LeasePaymentReport.GetMIRCTranInfo.GetTransactionInfo(
                                            argoLPItem.Region,
                                            argoLPItem.Office,
                                            argoLPItem.TranSeq,
                                            argoLPItem.Cashbox,
                                            argoLPItem.ProcDate);

                //filter out rescans (Gets highest sequence per GUID)
                leasePaymentTransactionList = leasePaymentTransactionList.GroupBy(p => p.GUID, (key, g) => g.OrderByDescending(y => y.sequence).First()).ToList();
                leasePaymentTransactionList.RemoveAll(x => x.REVERSED == 1);

                foreach (var localItem in leasePaymentTransactionList)
                {
                    if(!tranOfIntrestList.Any(x => x.GUID == localItem.GUID))
                    {
                        tranOfIntrestList.Add(localItem);
                    }
                }
            }

            var outputList = new List<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportFormatedModel>();

            foreach(var item in tranOfIntrestList)
            {
                var holder = new FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportFormatedModel();

                if(item.account.Trim(' ') == "40369779" && item.CRDR == "Cr")
                {
                    var blobThing = argoBLOBLeasePayments.Where(x => int.Parse(x.TranSeq) == item.transeq).FirstOrDefault();

                    holder.UtilCustAccountNumber = blobThing.UtilCustAccountNumber;
                    holder.UtilLeaCustomerName = blobThing.UtilLeaCustomerName;
                    holder.ProcDate = blobThing.ProcDate.ToString();
                    holder.Amount = blobThing.Amount;
                    holder.Region = blobThing.Region;
                    holder.Office = blobThing.Office;
                    holder.Cashbox = blobThing.Cashbox;
                    holder.TranSeq = blobThing.TranSeq;
                    holder.MsgSeq = blobThing.MsgSeq;
                    holder.OperatorID = blobThing.OperatorID;
                    holder.NAME = blobThing.NAME;
                    holder.field6 = item.field6;
                    holder.serial = item.serial;
                    holder.GUID = item.GUID;
                    holder.ISN = item.ISN;
                    holder.chgDateTime = item.chgDateTime.ToString();
                    holder.source = item.source;
                    holder.CRDR = item.CRDR;
                    holder.itemnumber = item.itemnumber.ToString();
                    holder.abanumber = item.abanumber;
                    holder.field4 = item.field4;
                    holder.account = item.account;
                    holder.trancode = item.trancode;
                    holder.Amount = item.amount.ToString();
                    holder.transeqSummeryNum = item.transeqSummeryNumber;
                }
                else
                {
                    //Non-Lease
                    holder.Region = item.region_id;
                    holder.Office = item.office_id;
                    holder.Region = item.region_id;
                    holder.Cashbox = item.cashsum_id;
                    //holder.TranSeq = item.transeq;
                    holder.GUID = item.GUID;
                    holder.TranSeq = item.transeq.ToString();
                    holder.ISN = item.ISN;
                    holder.chgDateTime = item.chgDateTime.ToString();
                    holder.OperatorID = item.oper_id;
                    holder.CRDR = item.CRDR;
                    holder.itemnumber = item.itemnumber.ToString();
                    holder.serial = item.serial;
                    holder.field6 = item.field6;
                    holder.abanumber = item.abanumber;
                    holder.field4 = item.field4;
                    holder.account = item.account;
                    holder.trancode = item.trancode;
                    holder.Amount = item.amount.ToString();
                    holder.transeqSummeryNum = item.transeqSummeryNumber;
                }

                outputList.Add(holder);
            }

            return outputList;
        }

        public static List<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportFormatedModel> Get(DateTime from, DateTime to)
        {
            var retVal = new List<FNBTellerETLADODB.LeasePaymentReport.LeasePaymentReportFormatedModel>();

            var totalDay = to.Subtract(from).Days;

            //Inclusive days
            for (int i = 0; i <= totalDay; i++)
            {
                retVal.AddRange(Get(from.AddDays(i)));
            }
            return retVal;
        }

        /// <summary>
        /// Reads in XML file produced from BPUEJFMT (ej formatter) and returns a list of the PAYMIC2 nodes that match a given criteria
        /// </summary>
        /// <param name="atThisDate"></param>
        /// <returns></returns>
        private static List<XElement> GetFilteredElementList(DateTime atThisDate)
        {
            var filteredList = new List<XElement>();
            string xmlFilePath = FileConfiguration.EjExtractUtil.OutputXMLDir.Value + atThisDate.ToString("yyyyMMdd") + ".xml";
            bool doesBPUEJFMTheadElementExist = false;

            if (FileConfiguration.Environment.Value == "LCL")
            {
                xmlFilePath = FileConfiguration.LeasePaymentReport.LCLHardcodedXMLLoc.Value;
            }

            using (XmlReader reader = XmlReader.Create(xmlFilePath))
            {
                reader.MoveToContent();

                if (reader.IsStartElement())
                {
                    if (reader.Name == "BPUEJFMT")
                    {
                        doesBPUEJFMTheadElementExist = true;
                    }
                }

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "PAYMIC2")
                        {
                            XElement el = XNode.ReadFrom(reader) as XElement;

                            if ((el.Element("MonetaryRq").Element("Tran").Element("Account").Element("Number").Value == "40369779") &&
                               (el.Element("MonetaryRq").Element("Tran").Element("Account").Element("TypeCode").Value == "LEA") &&
                               (el.Element("MonetaryRq").Element("Tran").Element("Account").Element("TypeDesc").Value == "Lease") &&
                               (el.Element("MonetaryRq").Element("Tran").Element("Payment").Element("Ticket").Element("TC").Value == "12"))
                            {
                                filteredList.Add(el);
                            }
                        }
                    }
                }
            }

            if (doesBPUEJFMTheadElementExist == false)
            {
                throw new Exception("No BPUEJFMT in XML");
            }

            return filteredList;
        }

        private static DateTime getDateTimeFromString(string input)
        {
            return new DateTime(int.Parse(input.Substring(0, 4)), int.Parse(input.Substring(4, 2)), int.Parse(input.Substring(6, 2)));
        }
    }
}
