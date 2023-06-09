using System;
using System.Text;

namespace FNBTellerETLADODB.MonetaryInstrumentLogReport
{
    public class MonetaryInstrumentLogModel
    {
        public string FundingTransactionNumber { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionTime { get; set; }
        public string CrDrInd { get; set; }
        public decimal? Amount { get; set; }
        public decimal? FeeAmount { get; set; }
        public string FeeFundedByCash { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionDescription { get; set; }
        public string Channel { get; set; }
        public string TellerId { get; set; }
        public string OfficeId { get; set; }
        public string BranchCode { get; set; }
        public string BranchState { get; set; }
        public string BranchZipCode { get; set; }
        public string SerialNumber { get; set; }
        public string FundingTransactionType { get; set; }
        public string FundingAccountTypeCode { get; set; }
        public string FundingAccountNumber { get; set; }
        public string FundingCheckNumber { get; set; }
        public string PayeeName { get; set; }
        public string Purchaser { get; set; }
        public string PurCustomerType { get; set; }
        public string PurInternalType { get; set; }
        public string PurOccupation { get; set; }
        public string PurAddress { get; set; }
        public string PurCity { get; set; }
        public string PurState { get; set; }
        public string PurZipCode { get; set; }
        public string PurCountry { get; set; }
        public string DateOfBirth { get; set; }
        public string IdType { get; set; }
        public string IdDescr { get; set; }
        public string IdNumber { get; set; }
        public string IdState { get; set; }
        public string IdIssuer { get; set; }
        public string TinType { get; set; }
        public string TinDescr { get; set; }
        public string TinNumber { get; set; }
        public string RecourseAcctNumber { get; set; }
        public string RecourseTransit { get; set; }
        public string RecourseAcctTypeCode { get; set; }
        public string RecourseHostAppCode { get; set; }
        public string RecourseHostRegionCode { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{TransactionDate.ToString("yyyy-MM-dd")},{Amount},{TransactionCode},");
            sb.Append($"{OfficeId},{BranchCode},{SerialNumber},");
            sb.Append($"{Purchaser.Replace(',', '.')},{PurCustomerType},{PurAddress.Replace(',', '.')},{PurCity},{PurState},{PurZipCode},{PurCountry},{DateOfBirth},{IdType},");
            sb.Append($"{IdDescr},=\"{IdNumber}\",{IdState},{IdIssuer.Replace(',', '.')},{TinType.Replace(',', '.')},{TinDescr.Replace(',', '.')},{TinNumber}");

            return sb.ToString();
        }
    }
}
