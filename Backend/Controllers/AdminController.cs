using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Backend.Helper;
using Backend.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace Backend.Controllers
{
    [MyAuthorize(Users = "Admin")]
    public class AdminController : Controller
    {
        private vapEntities1 db = new vapEntities1();
        /*private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
*/
         public AdminController()
            : this(new UserManager<AspNetUser>(new UserStore<AspNetUser>(new vapEntities1())))
        {
        }

         public AdminController(UserManager<AspNetUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<AspNetUser> UserManager { get; private set; }

        //

       /* public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }*/

        // GET: Admin
        public ActionResult Users()
        {
            /*if (User.Identity.Name != CqConstants.AdminAccount)
            {
                return RedirectToAction("Index", "NoAuth");
            }*/
            return View(db.AspNetUsers.ToList());
        }


        // GET: Admin/Create
        public ActionResult CreateUser()
        {
            return View(new AspNetUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(AspNetUser user,string roleType)
        {

            if (ModelState.IsValid)
            {
                user.Email = user.UserName;
                var result = UserManager.Create(user, VapLib.Constants.DefaultPass);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, roleType);
                    var record = db.AspNetUsers.Find(user.Id);
                    //record.Status = CqConstants.UserStatusEnum.Active.ToString();
                    //record.KsId = user.KsId;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Users");
                }
                AddErrors(result);
            }

            return View(user);
        }
        public ActionResult EditUser(string uid)
        {
            var record = db.AspNetUsers.Find(uid);
            if (record != null)
            {
                var identityUserRole = record.Roles.FirstOrDefault();
                if (identityUserRole != null)
                {
                    var roleid = identityUserRole.RoleId;
                    ViewBag.RoleType = db.AspNetRoles.Find(roleid).Name;
                }
                return View(record);
            }
            else
            {
                return RedirectToAction("Users");
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(AspNetUser user, string roleType,string oldRole)
        {

            if (ModelState.IsValid)
            {
                
                var record = db.AspNetUsers.Find(user.Id);
                if (record!=null)
                {
                    if (roleType != oldRole)
                    {
                        if(!string.IsNullOrEmpty(oldRole))
                            UserManager.RemoveFromRole(user.Id, oldRole);
                        UserManager.AddToRole(user.Id, roleType);
                    }

                    //record.KsId = user.KsId;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    
                }
                return RedirectToAction("Users");
            }

            return View(user);
        }
        public ActionResult ResetPassword(string uid)
        {

            var user = UserManager.FindById(uid);
            if (user != null)
            {
                /*string code = UserManager.GeneratePasswordResetToken(user.Id);
                var result = UserManager.ResetPassword(user.Id, code, CqConstants.DefaultPass);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                }*/

            }
            return RedirectToAction("Users");
        }
        public ActionResult MetaIndex(string metaType, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            var metas = db.SystemSettings.Where(c=>c.Id>0);
            if (!String.IsNullOrEmpty(metaType))
            {
                metas = metas.Where(s => s.Key == metaType);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                metas = metas.Where(s => s.Key.Contains(searchString));
            }
            return View(metas.OrderByDescending(c => c.Id).ToPagedList(page ?? 1, VapLib.Constants.PageSize));

        }


        // GET: PublicMetas/Edit/5
        public ActionResult EditMeta(string type, int? id)
        {
            var meta = new SystemSetting();
            if (id != null)
                meta = db.SystemSettings.Find(id);
            else
            {
                meta.Key = type;
            }

            return View(meta);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMeta(SystemSetting model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var record = db.SystemSettings.Find(model.Id);
                    record.Key = model.Key;
                    record.Value = model.Value;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var record = new SystemSetting()
                    {
                        Value = model.Value,
                        Key = model.Key
                    };
                    db.SystemSettings.Add(record);
                    db.SaveChanges();
                }

                return RedirectToAction("MetaIndex");
            }
            return View(model);
        }

        // GET: PublicMetas/Delete/5
        public ActionResult DeleteMeta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSetting publicMeta = db.SystemSettings.Find(id);
            if (publicMeta != null)
            {
                db.SystemSettings.Remove(publicMeta);
                db.SaveChanges();
            }
            return RedirectToAction("MetaIndex");
        }



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
