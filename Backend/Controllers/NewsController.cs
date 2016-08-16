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

namespace Backend.Controllers
{
    [MyAuthorize(Roles = "Admin,ClientService")]
    public class NewsController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        // GET: /News/
        public ActionResult Index(string newsTitle,string newsType)
        {
            if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            var news = db.News.Where(c=>c.Id>0);
            if (!string.IsNullOrEmpty(newsTitle))
            {
                news = news.Where(c => c.Title.Contains(newsTitle));
            }
            if (!string.IsNullOrEmpty(newsType))
            {
                news = news.Where(c => c.Type==newsType);
            }

            ViewBag.newsTitle = newsTitle;
            ViewBag.newsType = newsType;
            return View(news.ToList());
        }

        

        // GET: /News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(News model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = User.Identity.Name;
                model.DateTime = DateTime.Now;
                model.Status = "";
                model.Content = Server.HtmlDecode(model.Content);
                db.News.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: /News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: /News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(News model)
        {
            if (ModelState.IsValid)
            {
                var record = db.News.Find(model.Id);
                record.Content = Server.HtmlDecode(model.Content);
                record.Title = model.Title;
                record.Type = model.Type;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

       

        // POST: /News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
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
