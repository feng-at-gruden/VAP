﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Backend.Models;
using VapLib;

namespace Backend.Controllers
{
    public class BaoDanController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        // GET: BaoDan
        public ActionResult PendingSells()
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var status =报单状态.未成交.ToString();
            var type =报单类型.卖出.ToString();
            var records = db.BaoDanTransactions.Include(b => b.Member).Where(c=>c.Status==status
                &&c.Type==type).OrderBy(c=>c.DateTime);
            return View(records.ToList());
        }


        public ActionResult ApproveSell(int id)
        {

            var sell = db.BaoDanTransactions.Find(id);
            if (sell != null)
            {
                //检查卖出报单状态和类型
                if (sell.Status == 报单状态.未成交.ToString() && sell.Type == 报单类型.卖出.ToString())
                {
                    var member = db.Members.Find(sell.Member.Id);
                    //更改状态 为已成交
                    sell.Status = 报单状态.已成交.ToString();
                    //增加会员现金冻结记录 周一解冻
                    var tempAmount = sell.Amount * sell.Price - sell.Fee;
                    member.CashTransactions.Add(new CashTransaction
                    {
                        DateTime = DateTime.Now,
                        Type = 现金交易类型.售币所得.ToString(),
                        Status = 现金状态.冻结.ToString(),
                        Amount = tempAmount,
                        Fee = 0m,
                        BaoDanTransactionId = sell.Id
                    });
                    //增加会员冻结现金
                    member.Cash2 += tempAmount;
                    db.Entry(member).State = EntityState.Modified;
                    db.Entry(sell).State = EntityState.Modified;

                    db.SaveChanges();
                    

                }
                else
                {
                    ModelState.AddModelError("", "该记录不是未成交的卖出报单。");
                    
                }

            }
            //无此记录，跳转到首页
            ModelState.AddModelError("", "该记录不存在。");
            TempData["ModelState"] = ModelState;
            return RedirectToAction("PendingSells");
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