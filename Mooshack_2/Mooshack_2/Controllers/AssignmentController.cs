using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshack_2.Controllers
{
    public class AssignmentController : Controller
    {
        // GET: Assignment
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult TeacherNewAssignmentPage()
        {
            return View();
        }
    }
}