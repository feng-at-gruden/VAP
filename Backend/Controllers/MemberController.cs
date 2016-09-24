using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Backend.Helper;
using Backend.Models;
using Microsoft.Ajax.Utilities;
using Member = Backend.Models.Member;

namespace Backend.Controllers
{
    [MyAuthorize(Roles = "Admin,ClientService,Secretary")]
    public class MemberController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        // GET: /Member/
        
        public ActionResult Index(string account, string status)
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var members = db.Members.Where(c => c.Id > 0);
            if (!string.IsNullOrEmpty(account))
            {
                members = members.Where(c => c.Email.Contains(account));
            }

            if (!string.IsNullOrEmpty(status))
            {
                members = members.Where(c => c.Status == status);
            }
            
            ViewBag.account = account;
            ViewBag.status = status;
            return View(members.ToList().Select(c=>new MemberViewModel(c)).ToList());
        }
        public ActionResult MemberTree()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            
            mebs = db.Members.ToList();
            ViewBag.jstree = LoadMembers(null);
           
            return View();
        }
        public ActionResult IpLogs(string memberAccount)
        {

            var logs = db.IPLogs.Where(c => c.Id > 0);
            if (!string.IsNullOrEmpty(memberAccount))
            {
                logs = logs.Where(c => c.Member.Email.Contains(memberAccount));
            }


            ViewBag.memberAccount = memberAccount;
            return View(logs.ToList());
        }
        public ActionResult ApproveMember(int id)
        {

            var member = db.Members.Find(id);
            if (member != null)
            {
                if (member.Status == VapLib.会员状态.正常.ToString())
                {
                    ModelState.AddModelError("", "该会员状态正常。");
                }
                else
                {
                    if (member.Status == VapLib.会员状态.待审核.ToString())
                    {
                        ModelState.AddModelError("", "会员批准成功。");
                    }
                    if (member.Status == VapLib.会员状态.锁定.ToString())
                    {
                        ModelState.AddModelError("", "会员解锁成功。");
                    }
                    member.Status = VapLib.会员状态.正常.ToString();
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                }
                

            }
            //无此记录
            else
            {
                ModelState.AddModelError("", "该记录不存在。");
            }

            TempData["ModelState"] = ModelState;
            return RedirectToAction("Index");
        }
        public ActionResult ApproveIdentity(int id)
        {

            var member = db.Members.Find(id);
            if (member != null)
            {
                if (member.Status != VapLib.会员状态.正常.ToString())
                {
                    ModelState.AddModelError("", "请先修改会员为正常状态。");
                }
                else
                {
                    if (member.IdSubmitted.HasValue&&member.IdSubmitted.Value)
                    {
                        member.IdApproved = true;
                        db.Entry(member).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.AddModelError("", "会员身份认证成功。");
                    }
                    else
                    {
                        ModelState.AddModelError("", "会员没有上传身份证信息。");
                    }

                }

            }
            //无此记录
            else
            {
                ModelState.AddModelError("", "该会员不存在。");
            }

            TempData["ModelState"] = ModelState;
            return RedirectToAction("Index");
        }
        List<Member> mebs = new List<Member>();

        private string LoadMembers(int? code)
        {
            var rst = mebs.Where(c=>c.Referral_Id==code).ToList();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rst.Count; i++)
            {
                sb.Append("{\"id\":\"" + rst[i].Id+"|"+ rst[i].Achievement+"|"+rst[i].MemberLevel.Level
                    + "|" + rst[i].Cash1 + "|" + rst[i].Coin1 + "|" + rst[i].RegisterTime.ToString("yyyy/MM/dd") + "|" + rst[i].Coin2);
                sb.Append("\",\"text\":\"" + rst[i].Email + "\"");
                //sb.Append("\",\"info\":\"" + rst[i].Achievement+"|"+rst[i].MemberLevel.Level + "\"");
                
                string t = this.LoadMembers(rst[i].Id);
                if (!string.IsNullOrEmpty(t))
                    sb.AppendFormat(",\"children\":[{0}]", t);
                if (i + 1 == rst.Count)
                    sb.Append("}");
                else
                    sb.Append("},");
            }
            return sb.ToString();

        }
        public ActionResult LockMember(int id)
        {

            var member = db.Members.Find(id);
            if (member != null)
            {
                member.Status = VapLib.会员状态.锁定.ToString();
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                ModelState.AddModelError("", "会员锁定成功。");
            }
            //无此记录
            else
            {
                ModelState.AddModelError("", "该记录不存在。");
            }

            TempData["ModelState"] = ModelState;
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
            /* var temp=
             db.Members.Where(c => c.Referral_Id == member.Id).Select(c => c.Email).ToList();
             ViewBag.subordinates = temp;*/
            return View(member);
        }

        // POST: /Member/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Member model)
        {
            if (ModelState.IsValid)
            {
                var record = db.Members.Find(model.Id);
                record.Password1 = model.Password1;
                record.Password2 = model.Password2;
                var currentLevel = db.MemberLevels.First(c => c.Level == model.MemberLevel.Level);
                record.MemberLevel_Id = currentLevel.Id;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
