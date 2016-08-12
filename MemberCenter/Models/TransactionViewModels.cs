using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemberCenter.Models
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
    }

    public class CashTopupViewModel
    {

        [Required(ErrorMessage = "请填写汇款金额")]
        [Display(Name = "汇款金额")]
        //[Range(10000, 200000, ErrorMessage = "金额输入有误")]
        public decimal Amount { get; set; }

        [DisplayFormat(DataFormatString = "{#:n2}")]
        public decimal Odd { get; set; }

        public String OddStr { get { return Odd.ToString().Replace("0.","."); } }


        [Required(ErrorMessage = "请选择汇款方式")]
        [Display(Name = "汇款方式")]
        public int BankInfoId { get; set; }


        [Display(Name = "备注")]
        public string Comment { get; set; }

        [Display(Name = "汇款凭证")]
        public string FileUrl { get; set; }

        [Display(Name = "汇款银行")]
        public IEnumerable<BankInfoViewModel> SysBankInfos { get; set; }
    }

    public class CashWithdrawViewModel
    {

        [Required(ErrorMessage = "请填写提现金额")]
        [Display(Name = "提现金额")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestAmount { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(30, ErrorMessage = "{0}长度不足{2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "交易密码")]
        public string Password { get; set; }

        [Display(Name = "当前可提资金")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AvailableAmount { get; set; }

        [Display(Name = "手续费")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal MaxWithdrawAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal MinWithdrawAmount { get; set; }


        public int BankInfoId { get; set; }

        [Display(Name = "提现记录")]
        public IEnumerable<CashWithdrawHistoryViewModel> WithdrawHistory { get; set; }
    }

    public class CashWithdrawHistoryViewModel
    {
        [Display(Name = "提现时间")]
        public DateTime WithdrawTime { get; set; }

        [Display(Name = "提现金额")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Amount { get; set; }

        [Display(Name = "手续费")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        [Display(Name = "银行")]
        public String Bank { get; set; }

        [Display(Name = "账户")]
        public String BankAccount { get; set; }

        [Display(Name = "状态")]
        public String Status { get; set; }
    }

    public class BankInfoViewModel
    {
        [Display(Name = "开户银行")]
        [Required(ErrorMessage = "请填写开户银行")]
        public string Bank { get; set; }

        [Display(Name = "帐户名")]
        [Required(ErrorMessage = "请填写帐户名")]
        public string Name { get; set; }

        [Display(Name = "银行帐号")]
        [Required(ErrorMessage = "请填写银行帐号")]
        [StringLength(19, MinimumLength = 19, ErrorMessage = "请输入19位银行帐号")]
        public string Account { get; set; }

        [Display(Name = "备注")]
        public string Description { get; set; }

        [Display(Name = "网银链接")]
        public string URL { get; set; }

        public int Id { get; set; }
    }

}