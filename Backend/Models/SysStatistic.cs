//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Backend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SysStatistic
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<decimal> BaoDanBuyAmount { get; set; }
        public Nullable<decimal> BaoDanSellAmount { get; set; }
        public Nullable<short> NewMemberAmount { get; set; }
        public Nullable<decimal> TotalCashTransactionAmount { get; set; }
        public Nullable<decimal> BeginCoinPrice { get; set; }
        public Nullable<decimal> EndCoinPrice { get; set; }
        public Nullable<decimal> MaxCoinPrice { get; set; }
        public Nullable<decimal> MinCoinPrice { get; set; }
    }
}
