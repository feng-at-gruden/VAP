using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Backend.Helper;

namespace Backend.Controllers
{
    [MyAuthorize(Roles = "Admin,Finance,ClientService,Secretary")]
    public class HomeController : Controller
    {
        
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
           
            return View();
        }
        public ActionResult UnAuthorize()
        {
            
            return View();
        }
        
        public ActionResult Contact()
        {
            
            return View();
        }
    }
}