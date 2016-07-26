using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Backend.Models;

namespace Backend.Controllers
{
    public class MemberController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        // GET: /Member/
        public ActionResult Index(string account,string uId, string status)
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var members = db.Members.Where(c=>c.Id>0);
            if (!string.IsNullOrEmpty(account))
            {
                members = members.Where(c => c.Email.Contains(account));
            }
            if (!string.IsNullOrEmpty(uId))
            {
                members = members.Where(c => c.Id.ToString() == uId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                members = members.Where(c => c.Status == status);
            }
            ViewBag.account = account;
            ViewBag.status = status;
            ViewBag.uId = uId;
            return View(members.ToList());
        }
        public ActionResult ApproveMember(int id)
        {

            var member = db.Members.Find(id);
            if (member != null)
            {
                member.Status = VapLib.会员状态.正常.ToString();
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                ModelState.AddModelError("", "会员批准成功。");
            }
            //无此记录
            ModelState.AddModelError("", "该记录不存在。");
            return RedirectToAction("Index");
        }
      


        // GET: /Member/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            ViewBag.Referral_Id = new SelectList(db.Members, "Id", "Email", member.Referral_Id);
            return View(member);
        }

        // POST: /Member/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Email,UserName,RealName,Password1,Password2,Password3,Cash1,Cash2,Point1,Point2,ChongXiao1,ChongXiao2,Coin1,Coin2,RegisterTime,Level,Achievement,Status,TiXianStatus,TiBiStatus,IdSubmitted,IdApproved,ApprovedBy,Referral_Id")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Referral_Id = new SelectList(db.Members, "Id", "Email", member.Referral_Id);
            return View(member);
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
