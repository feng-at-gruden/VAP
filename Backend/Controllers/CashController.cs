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
    public class CashController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        // GET: /Cash/
        public ActionResult Index()
        {
            var cashtransactions = db.CashTransactions.Include(c => c.BankInfo).Include(c => c.Member).Include(c => c.PaymentMethod);
            return View(cashtransactions.ToList());
        }

       


        // GET: /Cash/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashTransaction cashtransaction = db.CashTransactions.Find(id);
            if (cashtransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankInfo_Id = new SelectList(db.BankInfos, "Id", "Bank", cashtransaction.BankInfo_Id);
            ViewBag.MemberId = new SelectList(db.Members, "Id", "Email", cashtransaction.MemberId);
            ViewBag.PaymentMethod_Id = new SelectList(db.PaymentMethods, "Id", "Bank", cashtransaction.PaymentMethod_Id);
            return View(cashtransaction);
        }

        // POST: /Cash/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,MemberId,Amount,DateTime,Type,Status,PaymentMethod_Id,BankInfo_Id")] CashTransaction cashtransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashtransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankInfo_Id = new SelectList(db.BankInfos, "Id", "Bank", cashtransaction.BankInfo_Id);
            ViewBag.MemberId = new SelectList(db.Members, "Id", "Email", cashtransaction.MemberId);
            ViewBag.PaymentMethod_Id = new SelectList(db.PaymentMethods, "Id", "Bank", cashtransaction.PaymentMethod_Id);
            return View(cashtransaction);
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
