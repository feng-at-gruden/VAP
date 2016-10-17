using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemberCenter.Models
{

    public class PointTransferViewModel
    {
        [Required(ErrorMessage = "请填写兑换券转账数量")]
        [Display(Name = "兑换券转账数量")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "请填写转账接收会员的UID或者邮箱")]
        [Display(Name = "接收会员")]
        public string User { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(30, ErrorMessage = "{0}长度不足{2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "交易密码")]
        public string Password { get; set; }

        [Display(Name = "当前可用兑换券")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AvailableAmount { get; set; }

        [Display(Name = "最大转账数量")]
        public Int32 MaxRequestAmount { get; set; }

        [Display(Name = "最少转账数量")]
        public int MinRequestAmount { get; set; }

    }

    public class PointBuyViewModel
    {
        [Display(Name = "可用现金")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AvailableCash { get; set; }

        [Display(Name = "购买数量")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestAmount { get; set; }
        
        [Display(Name = "最大购买数量")]
        public Int32 MaxRequestAmount { get; set; }

        [Display(Name = "最小购买数量")]
        public Int32 MinRequestAmount { get; set; }

        [Display(Name = "余额")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal CashLeft { get; set; }
    }


}