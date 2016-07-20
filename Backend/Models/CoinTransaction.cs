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
    
    public partial class CoinTransaction
    {
        public int Id { get; set; }
        public System.TimeSpan DateTime { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public short Type { get; set; }
        public short Status { get; set; }
        public int MemberId { get; set; }
        public decimal Fee { get; set; }
        public int PointTransaction_Id { get; set; }
        public int ChongXiaoTransaction_Id { get; set; }
        public int CashTransaction_Id { get; set; }
    
        public virtual CashTransaction CashTransaction { get; set; }
        public virtual ChongXiaoTransaction ChongXiaoTransaction { get; set; }
        public virtual Member Member { get; set; }
        public virtual PointTransaction PointTransaction { get; set; }
    }
}
