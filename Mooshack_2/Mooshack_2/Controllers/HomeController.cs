using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshack_2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

        public ActionResult TeacherFrontPage()
        {
            return View();
        }

        public ActionResult AdminFrontPage()
        {
            return View();
        }

        public ActionResult StudentFrontPage()
        {
            return View();
        }
    }
}