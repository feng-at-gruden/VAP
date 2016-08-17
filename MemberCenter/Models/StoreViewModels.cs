using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MemberCenter.Models
{
    public class ConsumptionViewModel
    {
        [Display(Name = "消费日期")]
        public DateTime DateTime { get; set; }

        [Display(Name = "消费数量")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Amount { get; set; }

        [Display(Name = "明细")]
        public string Comment { get; set; }

        [Display(Name = "类型")]
        public string Type { get; set; }

    }
}