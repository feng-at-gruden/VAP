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
    
    public partial class LockedCoin
    {
        public int Id { get; set; }
        public Nullable<decimal> LastPrice { get; set; }
        public Nullable<decimal> NextPrice { get; set; }
        public Nullable<decimal> Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LockedAmount { get; set; }
        public decimal AvailabeAmount { get; set; }
        public int MemberId { get; set; }
        public int BaoDanTransaction_Id { get; set; }
    
        public virtual BaoDanTransaction BaoDanTransaction { get; set; }
        public virtual Member Member { get; set; }
    }
}
