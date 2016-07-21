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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
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
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.Members.Count(m => m.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase)) > 0)
                {
                    ModelState.AddModelError("", "邮箱已被注册!");
                    return View(model);
                }

                //Check Referrall user is not null;
                int referralId = Int32.Parse(model.Referral);
                Member referral = db.Members.SingleOrDefault(m => m.Id == referralId);
                if(referral==null)
                {
                    ModelState.AddModelError("", "请输入正确推荐人ID!");
                    return View(model);
                }
                try
                {
                    Member newUser = new Member
                    {
                        Email = model.Email,
                        Password1 = model.Password,
                        Password2 = model.Password,
                        Password3 = model.Password,
                        RegisterTime = DateTime.Now,
                        Referral = referral,
                        Level = 用户等级.一星.ToString(),
                        Status = 会员状态.待审核.ToString(),
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
                    

                return RedirectToAction("Login");
            }

            return View(model);
        }


        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult MyAssets()
        {
            MyAssetViewModel model = new MyAssetViewModel()
            {
                AvailableCash = CurrentUser.Cash1,
                LockedCash = CurrentUser.Cash2,
                AvailablePoints = CurrentUser.Point1,
                LockedPoints = CurrentUser.Point2,
                AvailableChongXiao = CurrentUser.ChongXiao1,
                LockedChongXiao = CurrentUser.ChongXiao2,
                AvailableCoin = CurrentUser.Coin1,
                LockedCoin = CurrentUser.Coin2,
            };
            return View(model);
        }

        public ActionResult MyMembers()
        {
            IEnumerable<MyMemberViewModel> members = from row in CurrentUser.MyMembers where !row.Email.Equals(CurrentUser.Email, StringComparison.InvariantCultureIgnoreCase) orderby row.RegisterTime
                                         select new MyMemberViewModel
                                         {
                                             Email = row.Email,
                                             UserName = row.UserName,
                                             RegisterTime = row.RegisterTime,
                                             Archievement = row.Achievement,
                                             Level = row.Level,
                                         };
            return View(members);
        }


        public ActionResult MyAccount()
        {
            IPLog iplog = CurrentUser.IPLog.OrderByDescending(m=>m.DateTime).Take(1).ToArray()[0];
            MyAccountViewModel model = new MyAccountViewModel
            {
                Id = CurrentUser.Id,
                Email = CurrentUser.Email,
                RealName = CurrentUser.RealName,
                UserName = CurrentUser.UserName,
                Level = CurrentUser.Level,
                MyCash = CurrentUser.Cash1 + CurrentUser.Cash2,
                MyCoins = CurrentUser.Coin1 + CurrentUser.Coin2,
                MyPoints = CurrentUser.Point1 + CurrentUser.Point2,
                MyMember = CurrentUser.MyMembers.Count(),
                Archievement = CurrentUser.Achievement,
                RegisterTime = CurrentUser.RegisterTime,
                LastLoginIP = iplog.IP,
                LastLoginTime = iplog.DateTime,
                MyLink = Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "/Account/Register?referral=" + CurrentUser.Id),
            };
            return View(model);
        }
        

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