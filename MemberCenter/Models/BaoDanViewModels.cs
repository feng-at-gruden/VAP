using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemberCenter.Models
{
    public class BaoDanBuyViewModel
    {
        [Display(Name = "联和通用积分实时价格")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal CurrentCoinPrice { get; set; }

        [Display(Name = "可用现金")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal AvailableCash { get; set; }

        [Display(Name = "报单价格")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestPrice { get; set; }

        [Display(Name = "报单个数")]
        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal RequestQuantity { get; set; }

        [Display(Name = "报单总额")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestCash { get; set; }

        [Display(Name = "手续费")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        [Display(Name = "消耗现金")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal TotalCostCash { get; set; }

        [Display(Name = "余额")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal CashLeft { get; set; }
    }

    public class LockedCoinsViewModel
    {
        public Int32 BaoDanId { get; set; }

        public DateTime BaoDanTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal BaoDanQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal BaoDanPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal LastPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal NextPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal LockedAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal AvailableAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal AmountLeft { get; set; }

        public String Status { get; set; }
    }

    public class BaoDanHistoryViewModel
    {
        public Int32 Id { get; set; }

        public DateTime BaoDanTime { get; set; }

        public String Status { get; set; }

        public String Type { get; set; }

        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal RequestQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestCash { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal RequestPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal FinalPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal FinalQuantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal FinalCash { get; set; }
    }


    public class BaoDanSellViewModel
    {
        [Display(Name = "未成交记录")]
        public IEnumerable<BaoDanHistoryViewModel> RecentPendingRequests { get; set; }

        [Display(Name = "当前价格")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal CurrentPrice { get; set; }

        [Display(Name = "卖出数量")]
        [RegularExpression("([0-9]*)", ErrorMessage = "卖出数量必须为整数")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public int RequestAmount { get; set; }

        [Display(Name = "当前可售数量")]
        [DisplayFormat(DataFormatString = "{0:n6}")]
        public decimal AvailabeAmount { get; set; }

        public int MaxAmount { get { return (int)Math.Floor(AvailabeAmount); } }

        [Display(Name = "手续费")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Fee { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(30, ErrorMessage = "{0}长度不足{2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "交易密码")]
        public string Password { get; set; }
    }

}

