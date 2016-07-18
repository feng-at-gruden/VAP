using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MemberCenter.Models;
using VAPModel;

namespace MemberCenter.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {

            return RedirectToAction("MyAssets", "Account");
        }





    }
}