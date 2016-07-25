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
    
    public partial class CashTransaction
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public decimal Fee { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime DateTime { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public Nullable<int> BaoDanTransactionId { get; set; }
        public Nullable<int> PaymentMethod_Id { get; set; }
        public Nullable<int> BankInfo_Id { get; set; }
    
        public virtual BankInfo BankInfo { get; set; }
        public virtual BaoDanTransaction BaoDanTransaction { get; set; }
        public virtual Member Member { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}
