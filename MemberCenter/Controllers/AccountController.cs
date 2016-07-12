using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using MemberCenter.Models;
using VAPModel;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private Model1Container db = new Model1Container();
    

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
                String status = 会员状态.正常.ToString();
                Member user = db.Members.SingleOrDefault(m => m.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase) 
                    && m.Password1.Equals(model.Password)
                    && m.Status.Equals(status));
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Email, false);
                    return RedirectToLocal(returnUrl);
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
                //Check Referrall user is not null;
                Member referral = db.Members.SingleOrDefault(m => m.Email.Equals(model.Referral, StringComparison.InvariantCultureIgnoreCase));
                if(referral!=null)
                {
                    Member newUser = new Member
                    {
                        Email = model.Email,
                        Password1 = model.Password,
                        RegisterTime = DateTime.Now,
                        Referral = referral,
                        Level = 用户等级.无等级.ToString(),
                        Status = 会员状态.待审核.ToString(),
                    };
                    db.Members.Add(newUser);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "请输入正确推荐人邮箱!");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
                db = null;
            }
            base.Dispose(disposing);
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