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
        AssignmentService _assignmentService = new AssignmentService();

        public AssignmentController()
        {
            _assignmentService = new AssignmentService();
        }

        // GET: Assignment
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult TeacherAssingmentPage(int? courseID)
        {
            if (courseID != null)
            {
                var _assignmentService = new AssignmentService();
                var _assignmentModels = _assignmentService.getAssignmentByCourseID(courseID);
                return View(_assignmentModels);

            }

            return View();
        }

        public ActionResult TeacherNewAssignmentPage()
        {
            return View();
        }
    }
}