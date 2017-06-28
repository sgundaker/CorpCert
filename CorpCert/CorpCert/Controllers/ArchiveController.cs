using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace L2Test.Controllers
{
    //This controller only exists to allow me to make a longer route for the archives so that Javascript will break on the test result pages.
    public class ArchiveController : Controller
    {
        // GET: Archive
        public ActionResult Index()
        {
            return Redirect("/Home/Manage");
        }

        [HttpPost]
        [Authorize]
        public ActionResult ReportCards(string filepath)
        {
            return File(filepath, "text/html");
        }
    }
}