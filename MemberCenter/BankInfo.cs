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
    
    public partial class BankInfo
    {
        public BankInfo()
        {
            this.CashTransaction = new HashSet<CashTransaction>();
        }
    
        public int Id { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public string Description { get; set; }
        public Nullable<int> MemberId { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual ICollection<CashTransaction> CashTransaction { get; set; }
    }
}
