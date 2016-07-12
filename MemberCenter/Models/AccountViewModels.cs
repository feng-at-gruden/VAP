﻿using System.ComponentModel.DataAnnotations;

namespace MemberCenter.Models
{

    public class LoginViewModel
    {
        [Required(ErrorMessage = "请输入{0}")]
        [Display(Name = "邮箱")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "验证码")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Captcha { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage="请输入{0}")]
        [Display(Name = "邮箱")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(30, ErrorMessage = "{0}长度不足{2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "两次输入密码不相符.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "请输入{0}")]
        [Display(Name = "推荐人")]
        [EmailAddress]
        public string Referral { get; set; }

        [Display(Name = "验证码")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Captcha { get; set; }

        [Display(Name = "同意条款")]
        public bool AcceptTerm { get; set; }
    }

    public class MyAssetViewModel
    {
        public decimal AvailableCash { get; set; }
        public decimal LockedCash { get; set; }
        public decimal TotalCash { get; set; }

        public decimal AvailablePoints { get; set; }
        public decimal LockedPoints { get; set; }
        public decimal TotalPoints { get; set; }

        public decimal AvailableChongXiao { get; set; }
        public decimal LockedChongXiao { get; set; }
        public decimal TotalChongXiao { get; set; }

        public decimal AvailableCoin { get; set; }
        public decimal LockedCoin { get; set; }
        public decimal TotalCoin { get; set; }
    }


}
