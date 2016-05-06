using Mooshack_2.Services;
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
            var _assignmentService = new AssignmentService();
            var _assignmentModels = _assignmentService.getAllAssignmentViewModels();
            return View(_assignmentModels);
        }
        
        public ActionResult TeacherNewAssignmentPage()
        {
            return View();
        }
    }
}