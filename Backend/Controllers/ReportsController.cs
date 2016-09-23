using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Backend.Helper;
using Backend.Models;

namespace Backend.Controllers
{
    [MyAuthorize(Roles = "Admin,Finance,ClientService,Secretary")]
    
    public class ReportsController : Controller
    {
        private vapEntities1 db = new vapEntities1();

        // GET: Reports
        public ActionResult GeneralReport()
        {
            var model = new GeneralReportViewModel(db);
            return View(model);
        }
    }
}