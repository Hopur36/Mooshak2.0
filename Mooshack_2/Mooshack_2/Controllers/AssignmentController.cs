using Mooshack_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshack_2.Models;
using Mooshack_2.Models.ViewModels;

namespace Mooshack_2.Controllers
{
    public class AssignmentController : Controller
    {
        AssignmentService _assignmentService;
        CourseService _courseService;

        public AssignmentController()
        {
            _assignmentService = new AssignmentService();
            _courseService = new CourseService();
        }

        // GET: Assignment
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult TeacherAssignmentPage(int? courseID)
        {
            if (courseID != null)
            {
                var _assignmentService = new AssignmentService();
                var _courseName = _courseService.getCourseViewModelByID(courseID);
                var _assignmentModels = new TeacherAssignmentViewModel
                    { CourseName = _courseName.Name,CourseID = Convert.ToInt32(courseID),
                    Assignments = _assignmentService.getAssignmentByCourseID(courseID) };
            
                    
                return View(_assignmentModels);

            }

            return View();
        }

        public ActionResult TeacherNewAssignmentPage()
        {
            return View();
        }

        public ActionResult TeacherAssignmentMilestonesPage(int assignmentID)
        {
            AssignmentViewModel _assignment = _assignmentService.GetAssignmentViewModelByID(assignmentID);
            return View(_assignment);
        }

        public ActionResult CreateAssignment(int CourseID)
        {
            CreateAssignmentViewModel _newAssignmentViewModel = new CreateAssignmentViewModel();
            _newAssignmentViewModel.CourseID = CourseID;
            return View(_newAssignmentViewModel);
        }

        [HttpPost]
        public ActionResult CreateAssignment(CreateAssignmentViewModel model)
        {
            _assignmentService.CreateAssignment(model);

            return RedirectToAction("TeacherFrontPage", "Home", new { courseID = model.CourseID});
        }

        public ActionResult DeleteAssignment(int assignmentID,int courseReturnID)
        {
            _assignmentService.DeleteAssignment(assignmentID);
            return RedirectToAction("TeacherAssignmentPage", "Assignment",new { courseID = courseReturnID});
        }
    }
}