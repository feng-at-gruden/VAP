//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MemberCenter
{
    using System;
    using System.Collections.Generic;
    
    public partial class LockedCoin
    {
        public int Id { get; set; }
        public decimal LastPrice { get; set; }
        public decimal NextPrice { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal LockedAmount { get; set; }
        public decimal AvailabeAmount { get; set; }
        public int MemberId { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual BaoDanTransaction BaoDanTransaction { get; set; }
    }
}
