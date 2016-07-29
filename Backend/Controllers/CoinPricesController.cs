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
    public class CoinPricesController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        // GET: CoinPrices
        public ActionResult Index()
        {
            return View(db.CoinPrices.ToList());
        }

      
        // GET: CoinPrices/Create
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CoinPrice model)
        {
            if (ModelState.IsValid)
            {
                model.DateTime = DateTime.Now;
                db.CoinPrices.Add(model);
                db.SaveChanges();
                //todo 触发可用币释放 逻辑
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
