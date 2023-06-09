using System;

namespace FNBTellerETLADODB.LrgDollarOverridesReport
{
    public class LargeDollarOverrideModel
    {
        public DateTime Procdate { get; set; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string OfficeId { get; set; }
        public string OfficeName { get; set; }
        public string CashboxId { get; set; }
        public int TranSeqNum { get; set; }
        public short MsgSeqNum { get; set; }        //stored as tinyint in database
        public string OverrideId { get; set; }
        public string OverrideName { get; set; }
        public string SourceOperId { get; set; }
        public string TellerName { get; set; }
        public string AcctNum { get; set; }
        public double? AmountOne { get; set; }      //stored as float in database
        public double? AmountTwo { get; set; }      //stored as float in database
        public string AvailableBalance { get; set; }    //pulled from EJ Extract Data (BPUEJFMT) file
        public string CurrentBalance { get; set; }      //pulled from EJ Extract Data (BPUEJFMT) file
        public string CustomerName { get; set; }        //pulled from EJ Extract Data (BPUEJFMT) file
    }
}
