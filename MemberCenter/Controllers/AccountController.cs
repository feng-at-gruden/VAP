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
        public ActionResult Register()
        {
            return View();
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