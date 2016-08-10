using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Backend.Helper;

namespace Backend.Controllers
{
    public class HomeController : Controller
    {
        [MyAuthorize(Roles = "Admin,Finance,CustomerService")]
        public ActionResult Index()
        {
            /*if (TempData.ContainsKey("ModelState"))
            {
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
            }
            return View();*/
          return  RedirectToAction("GeneralReport","Reports");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}