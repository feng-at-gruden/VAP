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
    
    public partial class BaoDanTransaction
    {
        public BaoDanTransaction()
        {
            this.ChongXiaoTransaction = new HashSet<ChongXiaoTransaction>();
            this.CashTransaction = new HashSet<CashTransaction>();
        }
    
        public int Id { get; set; }
        public System.DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int MemberId { get; set; }
        public decimal Fee { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual PointTransaction PointTransaction { get; set; }
        public virtual LockedCoin LockedCoin { get; set; }
        public virtual ICollection<ChongXiaoTransaction> ChongXiaoTransaction { get; set; }
        public virtual ICollection<CashTransaction> CashTransaction { get; set; }
    }
}
