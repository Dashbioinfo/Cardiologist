using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CardiologistV2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Group of senior students in AIU";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "For more information or suggestions please do not hesitate to call : 0116114393 AIU";

            return View();
        }
    }
}
