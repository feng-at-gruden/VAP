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
    [MyAuthorize(Roles = "Admin")]
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
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            return View(db.AspNetUsers.ToList());
        }


        // GET: Admin/Create
        public ActionResult CreateUser()
        {
            return View(new AspNetUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(AspNetUser user, string roleType)
        {

            if (ModelState.IsValid)
            {
                user.Email = user.UserName;
                var result = UserManager.Create(user, VapLib.Constants.DefaultPass);
                if (result.Succeeded)
                {
                    //UserManager.AddToRole(user.Id, roleType);
                    var record = db.AspNetUsers.Find(user.Id);
                    //record.Status = CqConstants.UserStatusEnum.Active.ToString();
                    //record.KsId = user.KsId;
                    var role = db.AspNetRoles.Find(roleType);
                    record.AspNetRoles.Add(role);
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
                var userRole = record.AspNetRoles.FirstOrDefault();
                if (userRole != null)
                {
                    //var roleid = userRole.Id;
                    ViewBag.RoleType = userRole.Name;
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
        public ActionResult EditUser(AspNetUser user, string roleType, string oldRole)
        {

            if (ModelState.IsValid)
            {

                var record = db.AspNetUsers.Find(user.Id);
                if (record != null)
                {
                    if (roleType != oldRole)
                    {

                        record.AspNetRoles.Clear();
                        var newrole = db.AspNetRoles.Find(roleType);
                        record.AspNetRoles.Add(newrole);

                        /*UserManager.RemoveFromRole(user.Id, oldRole);
                    UserManager.AddToRole(user.Id, roleType);*/
                    }

                    //record.KsId = user.KsId;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.AddModelError("", "修改成功。");
                    TempData["ModelState"] = ModelState;


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
                UserManager.RemovePassword(uid);
                UserManager.AddPassword(uid, VapLib.Constants.DefaultPass);
                /*string code = UserManager.GeneratePasswordResetToken(user.Id);
                var result = UserManager.ResetPassword(user.Id, code, CqConstants.DefaultPass);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                }*/

                ModelState.AddModelError("", "密码重置成功。");
                TempData["ModelState"] = ModelState;


            }
            return RedirectToAction("Users");
        }
        public ActionResult MetaIndex()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var metas = db.SystemSettings.Where(c => c.Id > 0);

            return View(metas.ToList());

        }


        // GET: PublicMetas/Edit/5
        public ActionResult EditMeta(int id)
        {
            var meta = db.SystemSettings.Find(id);
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
                    //record.Key = model.Key;
                    record.Value = model.Value;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    ModelState.AddModelError("", "修改成功。");

                    TempData["ModelState"] = ModelState;
                }
                /*else
                {
                    var record = new SystemSetting()
                    {
                        Value = model.Value,
                        Key = model.Key
                    };
                    db.SystemSettings.Add(record);
                    db.SaveChanges();
                }*/

                return RedirectToAction("MetaIndex");
            }
            return View(model);
        }

        /*// GET: PublicMetas/Delete/5
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
        }*/
        public ActionResult Banks()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var type = VapLib.银行账户信息类型.系统账户.ToString();
            var bankInfoes = db.BankInfoes.Include(b => b.Member).Where(c => c.Type == type);
            return View(bankInfoes.ToList());
        }



        // GET: BankInfoes/Create
        public ActionResult CreateBank()
        {
            return View();
        }

        // POST: BankInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBank(BankInfo bankInfo)
        {
            if (ModelState.IsValid)
            {
                bankInfo.Type = VapLib.银行账户信息类型.系统账户.ToString();
                db.BankInfoes.Add(bankInfo);
                db.SaveChanges();
                ModelState.AddModelError("", "添加成功。");

                TempData["ModelState"] = ModelState;
                return RedirectToAction("Banks");
            }

            return View(bankInfo);
        }

        // GET: BankInfoes/Edit/5
        public ActionResult EditBank(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankInfo bankInfo = db.BankInfoes.Find(id);
            if (bankInfo == null)
            {
                return HttpNotFound();
            }
            return View(bankInfo);
        }

        // POST: BankInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBank(BankInfo bankInfo)
        {
            if (ModelState.IsValid)
            {
                var record = db.BankInfoes.Find(bankInfo.Id);
                record.Bank = bankInfo.Bank;
                record.Name = bankInfo.Name;
                record.Account = bankInfo.Account;
                record.URL = bankInfo.URL;
                record.Description = bankInfo.Description;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                ModelState.AddModelError("", "修改成功。");

                TempData["ModelState"] = ModelState;
                return RedirectToAction("Banks");
            }
            return View(bankInfo);
        }


        
        public ActionResult DeleteBank(int id)
        {
            BankInfo bankInfo = db.BankInfoes.Find(id);
            db.BankInfoes.Remove(bankInfo);
            db.SaveChanges();
            ModelState.AddModelError("", "删除操作成功。");

            TempData["ModelState"] = ModelState;
            return RedirectToAction("Banks");
        }

        public ActionResult UnlockCash()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            return View();
        }

        public ActionResult UnlockCashTrans()
        {
            var date = DateTime.Today.Date;
            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                date = date.AddDays(-1);

            }
            /*(DateTime.Today.DayOfWeek !=DayOfWeek.Monday)
            {
                ModelState.AddModelError("", "解冻操作只能在每周一执行。");
            }*/

            //var type = VapLib.现金交易类型.售币所得.ToString();
            var status = VapLib.现金状态.冻结.ToString();
            var lockTrans = db.CashTransactions.Where(c => c.DateTime < date
                && c.Status == status).ToList();
            foreach (var cashTransaction in lockTrans)
            {
                var member = db.Members.Find(cashTransaction.MemberId);
                member.Cash1 += cashTransaction.Amount;
                member.Cash2 -= cashTransaction.Amount;
                cashTransaction.Status = VapLib.现金状态.解冻.ToString();

                db.Entry(member).State = EntityState.Modified;
                db.Entry(cashTransaction).State = EntityState.Modified;
                db.SaveChanges();

            }
            ModelState.AddModelError("", "解冻操作执行成功。");

            TempData["ModelState"] = ModelState;
            return RedirectToAction("UnlockCash");
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
