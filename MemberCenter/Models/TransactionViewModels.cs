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
        public decimal Amount { get; set; }


        [Required(ErrorMessage = "请选择汇款方式")]
        [Display(Name = "汇款方式")]
        public PaymentMethodViewModel PaymentMethod { get; set; }


        [Display(Name = "备注")]
        public string Comment { get; set; }

        [Display(Name = "汇款凭证")]
        public string FileUrl { get; set; }


        public List<PaymentMethodViewModel> PaymentMethods { get; set; }
    }


    public class PaymentMethodViewModel
    {
        public string Bank { get; set; }
        public string Account { get; set; }
        public string Description { get; set; }
    }


    public class CashWithdrawViewModel
    {

        [Required(ErrorMessage = "请填写提现金额")]
        [Display(Name = "提现金额")]
        public decimal Amount { get; set; }

        public PaymentMethodViewModel PaymentMethod { get; set; }
    }

}