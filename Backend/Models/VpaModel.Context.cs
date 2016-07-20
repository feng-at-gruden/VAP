﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VapEntities : DbContext
    {
        public VapEntities()
            : base("name=vapEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CashTransaction> CashTransactions { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<PointTransaction> PointTransactions { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<ChongXiaoTransaction> ChongXiaoTransactions { get; set; }
        public virtual DbSet<CoinTransaction> CoinTransactions { get; set; }
        public virtual DbSet<BankInfo> BankInfos { get; set; }
        public virtual DbSet<CoinPrice> CoinPrices { get; set; }
        public virtual DbSet<IPLog> IPLogs { get; set; }
        public virtual DbSet<LockedCoin> LockedCoins { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<RefoundRate> RefoundRates { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
    }
}
