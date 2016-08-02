using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemberCenter.Models;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DashBoard()
        {
            return RedirectToAction("MyAssets", "Account");
        }


    }
}