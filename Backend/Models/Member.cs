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
    
    public partial class Member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Member()
        {
            this.BankInfos = new HashSet<BankInfo>();
            this.BaoDanTransactions = new HashSet<BaoDanTransaction>();
            this.CashTransactions = new HashSet<CashTransaction>();
            this.ChongXiaoTransactions = new HashSet<ChongXiaoTransaction>();
            this.IPLogs = new HashSet<IPLog>();
            this.LockedCoins = new HashSet<LockedCoin>();
            this.Members1 = new HashSet<Member>();
            this.PointTransactions = new HashSet<PointTransaction>();
        }
    
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public decimal Cash1 { get; set; }
        public decimal Cash2 { get; set; }
        public decimal Point1 { get; set; }
        public decimal Point2 { get; set; }
        public decimal ChongXiao1 { get; set; }
        public decimal ChongXiao2 { get; set; }
        public decimal Coin1 { get; set; }
        public decimal Coin2 { get; set; }
        public System.DateTime RegisterTime { get; set; }
        public decimal Achievement { get; set; }
        public string Status { get; set; }
        public string TiXianStatus { get; set; }
        public string TiBiStatus { get; set; }
        public Nullable<bool> IdSubmitted { get; set; }
        public Nullable<bool> IdApproved { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<int> Referral_Id { get; set; }
        public int MemberLevel_Id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BankInfo> BankInfos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaoDanTransaction> BaoDanTransactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CashTransaction> CashTransactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChongXiaoTransaction> ChongXiaoTransactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IPLog> IPLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LockedCoin> LockedCoins { get; set; }
        public virtual MemberLevel MemberLevel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member> Members1 { get; set; }
        public virtual Member Member1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PointTransaction> PointTransactions { get; set; }
    }
}
