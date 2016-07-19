using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MemberCenter.Models
{
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