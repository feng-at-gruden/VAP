using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MemberCenter.Models;
using MemberCenter.Helper;
using VapLib;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("验证码错误")]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Member user = db.Members.SingleOrDefault(m => m.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) 
                    && m.Password1.Equals(model.Password));
                if (user != null)
                {
                    if (user.Status.Equals(会员状态.正常.ToString()))
                    {
                        FormsAuthentication.SetAuthCookie(user.Email, false);
                        //Save Member login log
                        String ip = NetworkHelper.GetClientIPv4Address();
                        String userClient = Request.Browser.Browser + " " + Request.Browser.Version + " (" + Request.Browser.Platform + ")";
                        db.IPLogs.Add(new IPLog
                        {
                            DateTime = DateTime.Now,
                            IP = ip,
                            Client = userClient,
                            MemberId = user.Id,
                        });
                        db.SaveChanges();
                        return RedirectToLocal(returnUrl);
                    }
                    else if (user.Status.Equals(会员状态.待审核.ToString()))
                    {
                        ModelState.AddModelError("", "用户待审核!");
                    }
                    else if (user.Status.Equals(会员状态.锁定.ToString()))
                    {
                        ModelState.AddModelError("", "帐号锁定，请与管理员联系!");
                    }
                }
                else 
                {
                    ModelState.AddModelError("", "用户名/密码错误!");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(int? referral)
        {
            return View(new RegisterViewModel { Referral = referral + "" });
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("验证码错误")]
        public ActionResult Register(RegisterViewModel model, int? referral)
        {
            if (ModelState.IsValid)
            {
                if(!model.AcceptTerm)
                {
                    ModelState.AddModelError("", "请阅读并同意用户协议!");
                    return View(model);
                }

                if (db.Members.Count(m => m.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase)) > 0)
                {
                    ModelState.AddModelError("", "邮箱已被注册!");
                    return View(model);
                }

                //Check Referrall user is not null;
                int referralId = Int32.Parse(model.Referral);
                Member myRef = db.Members.SingleOrDefault(m => m.Id == referralId);
                if (myRef == null)
                {
                    ModelState.AddModelError("", "请输入正确推荐人ID!");
                    return View(model);
                }
                try
                {
                    String initLevelStr = 会员等级.无等级.ToString();
                    MemberLevel initLevle = db.MemberLevel.SingleOrDefault(m => m.Level.Equals(initLevelStr, StringComparison.InvariantCultureIgnoreCase));
                    if(initLevle==null)
                    {
                        ModelState.AddModelError("", "系统用户等级参数错误!");
                        return View(model);
                    }
                    Member newUser = new Member
                    {
                        Email = model.Email,
                        Password1 = model.Password,
                        Password2 = model.Password,
                        RegisterTime = DateTime.Now,
                        Referral = myRef,
                        MemberLevel = initLevle,
                        //Status = 会员状态.待审核.ToString(),
                        Status = 会员状态.正常.ToString(),
                        Cash1 = 0,
                        Cash2 = 0,
                        Coin1 = 0,
                        Coin2 = 0,
                        Point1 = 0,
                        Point2 = 0,
                        ChongXiao1 = 0,
                        ChongXiao2 = 0,
                        Achievement = 0,                         
                    };
                    db.Members.Add(newUser);
                    db.SaveChanges();
                    ViewBag.ActionMessage = "注册成功！请返回登录。";
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                                
                        }
                    }
                }

                return View(new RegisterViewModel());
                //return RedirectToAction("Login");
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff(string returnUrl)
        {
            FormsAuthentication.SignOut();
            if (returnUrl!=null)
                return RedirectToLocal(returnUrl);
            else
                return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/MyAssets
        public ActionResult MyAssets()
        {
            MyAccountViewModel model = GetMyAccountViewModel(CurrentUser);
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // GET: /Account/MyMembers
        public ActionResult MyMembers()
        {
            IEnumerable<MyMemberViewModel> members = from row in CurrentUser.MyMembers where !row.Email.Equals(CurrentUser.Email, StringComparison.InvariantCultureIgnoreCase) orderby row.RegisterTime
                                         select new MyMemberViewModel
                                         {
                                             Email = row.Email,
                                             UserName = row.UserName,
                                             RegisterTime = row.RegisterTime,
                                             Achievement = row.Achievement,
                                             Level = row.MemberLevel.Level,
                                         };
            SetMyAccountViewModel();
            return View(members);
        }

        //
        // GET: /Account/MyAccount
        public ActionResult MyAccount()
        {
            IPLog iplog = CurrentUser.IPLog.OrderByDescending(m=>m.DateTime).Take(1).ToArray()[0];
            String sType = 现金交易类型.购买积分.ToString();
            MyAccountInfoViewModel model = new MyAccountInfoViewModel
            {
                Id = CurrentUser.Id,
                Email = CurrentUser.Email,
                Mobile = CurrentUser.Mobile,
                RealName = CurrentUser.RealName,
                UserName = CurrentUser.UserName,
                Level = CurrentUser.MemberLevel.Level,
                MyCash = CurrentUser.Cash1 + CurrentUser.Cash2,
                MyCoins = CurrentUser.Coin1 + CurrentUser.Coin2,
                MyPoints = CurrentUser.Point1 + CurrentUser.Point2,
                MyBaoDanCash = Math.Abs(CurrentUser.CashTransaction.Where(m=>m.Type == sType).Sum(m=>m.Amount)),
                MyChongXiao = CurrentUser.ChongXiao1 + CurrentUser.ChongXiao2,
                MyMember = CurrentUser.MyMembers.Count(),
                Achievement = CurrentUser.Achievement,
                RegisterTime = CurrentUser.RegisterTime,
                LastLoginIP = iplog.IP,
                LastLoginTime = iplog.DateTime,
                MyLink = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, SiteConfigurationHelper.SiteRootPath + "/Account/Register?referral=" + CurrentUser.Id),
            };
            
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // GET: /Account/EditMyAccount
        public ActionResult EditMyAccount()
        {
            MyAccountInfoViewModel model = new MyAccountInfoViewModel
            {
                RealName = CurrentUser.RealName,
                UserName = CurrentUser.UserName,
                Mobile = CurrentUser.Mobile,
                IdentifyFile = CurrentUser.IdentityPath,
            };
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // POST: /Account/EditMyAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMyAccount(MyAccountInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var requestHelper = new RequestHelper(this.Request);
                var filePath = requestHelper.SaveImageToServer(GetSystemSettingString("MemberUploadIdentityFilePath"), false);
                CurrentUser.UserName = model.UserName;
                CurrentUser.RealName = model.RealName;
                CurrentUser.Mobile = model.Mobile;
                CurrentUser.IdentityPath = filePath != "" ? filePath : CurrentUser.IdentityPath;

                db.SaveChanges();
                ViewBag.ActionMessage = "个人资料更新成功!";
                TempData["ActionMessage"] = ViewBag.ActionMessage;
                return RedirectToAction("Success", "Message");
            }
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // GET: /Account/SecureSetting
        public ActionResult SecureSetting()
        {
            SecureSettingViewModel model = new SecureSettingViewModel
            {
                BankInfoAdded = CurrentUser.BankInfo.Count >0,
                BankInfo = (from row in CurrentUser.BankInfo
                            orderby row.Id descending
                            select new BankInfoViewModel
                            {
                                Account = row.Account,
                                Name = row.Name,
                                Bank = row.Bank,
                                Description = row.Description,
                                Id = row.Id,
                            }).SingleOrDefault(),
            };
            if (TempData["ModelState"]!=null)
            {
                ModelStateDictionary lastModelState = (ModelStateDictionary)TempData["ModelState"];
                foreach(var key in lastModelState.Keys)
                {
                    String error = GetErrorMessageForKey(lastModelState, key);
                    if (error!=null)
                        ModelState.AddModelError(key, error);
                }
            }
            if (TempData["ActionMessage"] != null)
            {
                ViewBag.ActionMessage = TempData["ActionMessage"];
                return RedirectToAction("Success", "Message");
            }
            SetMyAccountViewModel();
            return View(model);
        }

        //
        // POST: /Account/UpdatePassword
        [HttpPost]
        public ActionResult UpdatePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                String pwdType;
                if (model.Type == "1")
                {
                    pwdType = "登录密码";
                    if (CurrentUser.Password1 != model.OldPassword)
                    {
                        ModelState.AddModelError("", "原密码错误");
                        //
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("SecureSetting");
                    }
                    CurrentUser.Password1 = model.Password;
                }
                else
                {
                    pwdType = "交易密码";
                    if (CurrentUser.Password2 != model.OldPassword)
                    {
                        ModelState.AddModelError("", "原密码错误");
                        //
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("SecureSetting");
                    }
                    CurrentUser.Password2 = model.Password;
                }
                    
                db.SaveChanges();
                ViewBag.ActionMessage = pwdType + "更新成功!";
                TempData["ActionMessage"] = ViewBag.ActionMessage;
                return RedirectToAction("Success", "Message");
            }
            TempData["ModelState"] = ModelState;
            SetMyAccountViewModel();
            return RedirectToAction("SecureSetting");
        }

        //
        // POST: /Account/BankInfo
        [HttpPost]
        public ActionResult BankInfo(BankInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(CurrentUser.BankInfo.Count<=0)
                {
                    CurrentUser.BankInfo.Add(new BankInfo
                    {
                        Bank = model.Bank,
                        Name = model.Name,
                        Account = model.Account,
                        Type = 银行账户信息类型.会员账户.ToString(),
                        Description = "",
                        URL = "",
                    });
                }
                else
                {
                    BankInfo newBankInfo = CurrentUser.BankInfo.SingleOrDefault();
                    newBankInfo.Bank = model.Bank;
                    newBankInfo.Name = model.Name;
                    newBankInfo.Account = model.Account;
                }
                
                db.SaveChanges();
                ViewBag.ActionMessage = "银行账户信息更新成功!";
                TempData["ActionMessage"] = ViewBag.ActionMessage;
                return RedirectToAction("Success", "Message");
            }
            TempData["ModelState"] = ModelState;
            SetMyAccountViewModel();
            return RedirectToAction("SecureSetting");
        }

        //
        // GET: /Account/IPLog
        public ActionResult IPLog()
        {
            IEnumerable<IPLogViewModel> model = from row in CurrentUser.IPLog
                                                orderby row.DateTime descending
                                                select new IPLogViewModel
                                                {
                                                    LoginIP = row.IP,
                                                    LoginTime = row.DateTime,
                                                    UserClient = row.Client,
                                                };
            SetMyAccountViewModel();
            return View(model);
        }

        #region Helpers
       
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public string GetErrorMessageForKey(ModelStateDictionary dictionary, string key)
        {
            return dictionary[key].Errors.Count>0? dictionary[key].Errors.First().ErrorMessage : null;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}