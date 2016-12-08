using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Backend.Models;
using Backend.Helper;

namespace Backend.Controllers
{
   
    public class CoinPricesController : Controller
    {
        private vapEntities1 db = new vapEntities1();
        private static object dbLock = new object();

        // GET: CoinPrices
        public ActionResult Index()
        {
            return View(db.CoinPrices.ToList());
        }

      
        // GET: CoinPrices/Create
         [MyAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var price = db.CoinPrices.OrderByDescending(m => m.DateTime).ToList();
            
            if (price.Any())
            {
                ViewBag.LastPrice = price.First().Price;
            }
            else
            {
                ViewBag.LastPrice = 0.0m;
            }

            return View();
        }

        // POST: CoinPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
         [MyAuthorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CoinPrice model, int type = 2)
        {
            if (ModelState.IsValid)
            {
                lock (dbLock)
                {
                    var newPrice = model.Price;
                    if (type == 1 || type == 2)
                    {
                        model.DateTime = DateTime.Now;

                        db.CoinPrices.Add(model);
                        db.SaveChanges();
                    }

                    // 触发可用币释放 逻辑
                    if (type == 1 || type == 3)
                    {
                        var lockRecords = db.LockedCoins.Where(m => m.LockedAmount > 0 && m.NextPrice <= newPrice);
                        decimal rate = SystemSettingHelper.GetSystemSettingDecimal(db, "CoinPriceRate");
                        foreach (var lockRecord in lockRecords)
                        {
                            if (newPrice >= lockRecord.NextPrice)
                            {
                                var member = db.Members.Find(lockRecord.MemberId);
                                decimal amount = lockRecord.LockedAmount * rate;
                                if (lockRecord.LockedAmount <= amount)
                                {
                                    amount = lockRecord.LockedAmount;
                                }

                                lockRecord.LockedAmount -= amount;
                                lockRecord.AvailabeAmount += amount;
                                lockRecord.LastPrice = newPrice;
                                lockRecord.NextPrice = Math.Ceiling(newPrice.Value * (1 + rate) * 1000) / 1000;
                                //lockRecord.NextPrice = Math.Round(currentPrice.Value + currentPrice.Value * SystemSettingHelper.GetSystemSettingDecimal(db, "CoinPriceRate"), 3);

                                member.Coin1 += amount;
                                member.Coin2 -= amount;

                                db.Entry(lockRecord).State = EntityState.Modified;
                                db.Entry(member).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }
        public ActionResult GetCoinPrices(int count)
        {
            var reportData = db.CoinPrices.OrderByDescending(c=>c.Id).Take(count).ToList().OrderBy(c=>c.Id).Select(c => new ReportData() {Title = c.DateTime.ToString("yy-MM-dd"), Price = c.Price.Value});
            return Json(new { Data = reportData }, JsonRequestBehavior.AllowGet); 
            
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
    public class ReportData
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}
