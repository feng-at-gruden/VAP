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

        //
        // GET: /Transaction/CashTopup
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

        //
        // POST: /Transaction/CashTopup
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

        //
        // GET: /Transaction/CashWithdraw
        public ActionResult CashWithdraw()
        {
            //TODO
            return View();
        }



        //
        // GET: /Transaction/History
        public ActionResult History()
        {
            IEnumerable<CashTransactionViewModel> model = from row in CurrentUser.CashTransaction
                                                        orderby row.DateTime
                                                          select new CashTransactionViewModel
                                                        {
                                                            Type = row.Type,
                                                            Status = row.Status,
                                                            DateTime = row.DateTime,
                                                            Fee = row.Fee,
                                                            Amount = row.Amount,
                                                            ID = row.Id,
                                                        };
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
