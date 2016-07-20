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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CashTransaction()
        {
            this.CoinTransactions = new HashSet<CoinTransaction>();
        }
    
        public int Id { get; set; }
        public int MemberId { get; set; }
        public decimal Amount { get; set; }
        public System.TimeSpan DateTime { get; set; }
        public short Type { get; set; }
        public short Status { get; set; }
        public int PaymentMethod_Id { get; set; }
        public int BankInfo_Id { get; set; }
    
        public virtual BankInfo BankInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CoinTransaction> CoinTransactions { get; set; }
        public virtual Member Member { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}
