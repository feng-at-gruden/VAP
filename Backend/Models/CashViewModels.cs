using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{

    public class CashTransactionViewModel
    {
        [Display(Name = "交易ID")]
        public int ID { get; set; }

        [Display(Name = "交易时间")]
        public DateTime DateTime { get; set; }

        [Display(Name = "状态")]
        public String Status { get; set; }

        [Display(Name = "资金类型")]
        public String Type { get; set; }

        [Display(Name = "交易金额")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Amount { get; set; }

        [Display(Name = "手续费")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        public int MemberId { get; set; }

        public string MemberEmail { get; set; }

        public string Bank { get; set; }

        public string RemitNumber { get; set; }

        public string RemitAccount { get; set; }

        public string FileUrl { get; set; }

        public string Comment { get; set; }
    }

    public class CashWithdrawHistoryViewModel
    {
        [Display(Name = "提现时间")]
        public DateTime WithdrawTime { get; set; }

        [Display(Name = "提现金额")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AmountDisp { get { return Math.Abs(Amount); } }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RealAmount { get { return Math.Abs(Amount) - Fee; } }

        [Display(Name = "手续费")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        [Display(Name = "银行")]
        public String Bank { get; set; }

        [Display(Name = "账户")]
        public String BankAccount { get; set; }

        [Display(Name = "状态")]
        public String Status { get; set; }

        public int Id { get; set; }

        public int MemberId { get; set; }

        public string MemberEmail { get; set; }

    }

}