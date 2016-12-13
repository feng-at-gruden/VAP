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
using VapLib;

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
                //资金总表/明细对账
                if (type == 5)
                {
                    string html = "<table class=\"gridtable\"><tr><td><b>UID</b></td><td>Cash1</td><td>Cash2</td><td>Cash1+Cash2</td><td>Transaction Sum</td></tr>";
                    foreach (var mb in db.Members.OrderByDescending(m => m.Id))
                    {

                        var sc = mb.Cash1 + mb.Cash2;
                        decimal tc = 0;
                        foreach (var bt in mb.CashTransactions)
                        {
                            tc += bt.Amount;
                        }
                        if (Math.Abs(sc - tc) > 100)
                        {
                            html += string.Format("<tr class=\"hight-light\" ><td><b>{0}</b></td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", mb.Id, mb.Cash1, mb.Cash2, mb.Cash1 + mb.Cash2, tc);
                        }
                        else
                        {
                            html += string.Format("<tr><td><b>{0}</b></td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", mb.Id, mb.Cash1, mb.Cash2, mb.Cash1 + mb.Cash2, tc);
                        }
                    }
                    html += "</table>";

                    ViewBag.ActionMessage = html;
                    TempData["ActionMessage"] = ViewBag.ActionMessage;

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

                //积分总表/明细对账
                if (type == 4 )
                {
                    string html = "<table class=\"gridtable\"><tr><td><b>UID</b></td><td>Coin1</td><td>Coin2</td><td>Coin1+Coin2</td><td>Transaction Sum</td><td>Locked Coins Sum</td></tr>";
                    foreach(var mb in db.Members.OrderByDescending(m=>m.Id))
                    {
                        var sc = mb.Coin1 + mb.Coin2;

                        decimal  tc = 0;
                        string s = 报单状态.用户已取消.ToString();
                        
                        foreach(var bt in mb.BaoDanTransactions.Where(m=>!s.Equals(m.Status)))
                        {
                            var amt = bt.Amount;
                            if(报单类型.卖出.ToString().Equals(bt.Type))
                                amt = -bt.Amount;
                            tc += amt;
                        }

                        decimal lcs = 0;
                        foreach(var lc in mb.LockedCoins)
                        {
                            lcs += lc.LockedAmount + lc.AvailabeAmount;
                        }

                        if (Math.Abs(tc - sc) > 100)
                        {
                            html += string.Format("<tr class=\"hight-light\" ><td><b>{0}</b></td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>", mb.Id, mb.Coin1, mb.Coin2, mb.Coin1 + mb.Coin2, tc, lcs);
                        }
                        else if (Math.Abs(lcs - sc) > 100)
                        {
                            html += string.Format("<tr class=\"hight-light-1\" ><td><b>{0}</b></td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>", mb.Id, mb.Coin1, mb.Coin2, mb.Coin1 + mb.Coin2, tc, lcs);
                        }
                        else
                        {
                            html += string.Format("<tr><td><b>{0}</b></td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>", mb.Id, mb.Coin1, mb.Coin2, mb.Coin1 + mb.Coin2, tc, lcs);
                        }

                    }
                    html += "</table>";

                    ViewBag.ActionMessage = html;
                    TempData["ActionMessage"] = ViewBag.ActionMessage;

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
