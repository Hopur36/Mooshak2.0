using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshack_2.Models;
using Mooshack_2.Services;
using Microsoft.AspNet.Identity;
using Mooshack_2.Models.ViewModels;

namespace Mooshack_2.Controllers
{
    public class HomeController : Controller
    {
        CourseService _courseService;
        AssignmentService _assignmentService;

        public HomeController()
        {
            _courseService = new CourseService();
            _assignmentService = new AssignmentService();
        }
        public ActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminFrontPage");
            }
            else if (User.IsInRole("Teacher"))
            {
                return RedirectToAction("TeacherFrontPage");
            }
            else if (User.IsInRole("Student"))
            {
                return RedirectToAction("StudentFrontPage");
            }

            return RedirectToAction("Login", "Account");
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

        [Authorize(Roles = "Teacher")]
        public ActionResult TeacherFrontPage()
        {
            var _courses = _courseService.getAllActiveCoursesByTeacherID(User.Identity.GetUserId());
            List<AssignmentViewModel> _allAssignments = new List<AssignmentViewModel>();
            foreach(CourseViewModel _course in _courses)
            {
                if(_assignmentService.getAssignmentByCourseID(_course.id) != null)
                {
                    foreach(AssignmentViewModel _assignment in _assignmentService.getAssignmentByCourseID(_course.id))
                    {
                        _allAssignments.Add(_assignment);
                    }
                }
            }
            var _teacherFrontPageViewModel = new TeacherFrontPageViewModel() { Courses = _courses, Assignments = _allAssignments };

            return View(_teacherFrontPageViewModel);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AdminFrontPage()
        {

            return View();
        }

        /// <summary>
        /// This function gets all active courses by StudentID and Assignments for those courses
        /// </summary>
        /// <returns>StudentFrontPageViewModel</returns>
        [Authorize(Roles = "Student")]
        public ActionResult StudentFrontPage()
        {
            var _courses = _courseService.getAllActiveCoursesByStudentID(User.Identity.GetUserId());
            List<AssignmentViewModel> _allAssignments = new List<AssignmentViewModel>();
            foreach (CourseViewModel _course in _courses)
            {
                if (_assignmentService.getAssignmentByCourseID(_course.id) != null)
                {
                    foreach (AssignmentViewModel _assignment in _assignmentService.getAssignmentByCourseID(_course.id))
                    {
                        _allAssignments.Add(_assignment);
                    }
                }
            }
            var _studentFrontPageViewModel = new StudentFrontPageViewModel() { Courses = _courses, Assignments = _allAssignments  };
            return View(_studentFrontPageViewModel);
        }
    }
}