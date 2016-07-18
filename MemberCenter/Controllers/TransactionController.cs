using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MemberCenter;
using MemberCenter.Models;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class TransactionController : BaseController
    {
        private Model1Container db = new Model1Container();


        public ActionResult CashTopup()
        {
            IQueryable<PaymentMethodViewModel> paymentMethods = from row in db.PaymentMethods
                                 select new PaymentMethodViewModel
                                 {
                                     Account = row.Account,
                                     Bank = row.Bank,
                                     Description = row.Description,
                                 };

            CashTopupViewModel model = new CashTopupViewModel {
                PaymentMethods = paymentMethods.ToList(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CashTopup(CashTopupViewModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO
                //db.Entry(cashtransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View();
        }


        public ActionResult CashWithdraw()
        {
            return View();
        }

        


        /////////////////////////////////////////////////////////


        // GET: /Transaction/
        public ActionResult Index()
        {
            var cashtransactions = db.CashTransactions.Include(c => c.Member).Include(c => c.PaymentMethod);
            return View(cashtransactions.ToList());
        }

        // GET: /Transaction/Details/5
        public ActionResult Details(int? id)
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
            return View(cashtransaction);
        }

        // GET: /Transaction/Create
        public ActionResult Create()
        {
            ViewBag.MemberId = new SelectList(db.Members, "Id", "Email");
            ViewBag.Id = new SelectList(db.PaymentMethods, "Id", "Bank");
            return View();
        }

        // POST: /Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,MemberId,Amount,DateTime,Type,Status")] CashTransaction cashtransaction)
        {
            if (ModelState.IsValid)
            {
                db.CashTransactions.Add(cashtransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MemberId = new SelectList(db.Members, "Id", "Email", cashtransaction.MemberId);
            ViewBag.Id = new SelectList(db.PaymentMethods, "Id", "Bank", cashtransaction.Id);
            return View(cashtransaction);
        }

        // GET: /Transaction/Edit/5
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
            ViewBag.MemberId = new SelectList(db.Members, "Id", "Email", cashtransaction.MemberId);
            ViewBag.Id = new SelectList(db.PaymentMethods, "Id", "Bank", cashtransaction.Id);
            return View(cashtransaction);
        }

        // POST: /Transaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,MemberId,Amount,DateTime,Type,Status")] CashTransaction cashtransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashtransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MemberId = new SelectList(db.Members, "Id", "Email", cashtransaction.MemberId);
            ViewBag.Id = new SelectList(db.PaymentMethods, "Id", "Bank", cashtransaction.Id);
            return View(cashtransaction);
        }

        // GET: /Transaction/Delete/5
        public ActionResult Delete(int? id)
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
            return View(cashtransaction);
        }

        // POST: /Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CashTransaction cashtransaction = db.CashTransactions.Find(id);
            db.CashTransactions.Remove(cashtransaction);
            db.SaveChanges();
            return RedirectToAction("Index");
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
