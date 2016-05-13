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
            _courseService = new CourseService(null);
            _assignmentService = new AssignmentService(null);
        }
        public ActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("showAllUsers", "Account" );
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
        public ActionResult TeacherFrontPage(string sortOrder)
        {
            var _courses = _courseService.getAllActiveCoursesByTeacherID(User.Identity.GetUserId());
            List<AssignmentViewModel> _allAssignments = new List<AssignmentViewModel>();
            foreach(CourseViewModel _course in _courses)
            {
                if(_assignmentService.getActiveAssignmentByCourseID(_course.id) != null)
                {
                    foreach(AssignmentViewModel _assignment in _assignmentService.getActiveAssignmentByCourseID(_course.id))
                    {
                        _allAssignments.Add(_assignment);
                    }
                }
            }
            _allAssignments.Sort((x,y) => x.EndDateTime.CompareTo(y.EndDateTime));
            var _teacherFrontPageViewModel = new TeacherFrontPageViewModel() { Courses = _courses, Assignments = _allAssignments };

            switch (sortOrder)
            {
                case "title":
                    _teacherFrontPageViewModel.Assignments = _teacherFrontPageViewModel.Assignments.OrderBy(x => x.Title).ToList();
                    break;
                case "course":
                    _teacherFrontPageViewModel.Assignments = _teacherFrontPageViewModel.Assignments.OrderBy(x => x.CourseName).ToList();
                    break;
                case "start":
                    _teacherFrontPageViewModel.Assignments = _teacherFrontPageViewModel.Assignments.OrderBy(x => x.StartDateTime).ToList();
                    break;
                case "end":
                    _teacherFrontPageViewModel.Assignments = _teacherFrontPageViewModel.Assignments.OrderBy(x => x.EndDateTime).ToList();
                    break;
                default:
                    _teacherFrontPageViewModel.Assignments = _teacherFrontPageViewModel.Assignments.OrderBy(x => x.EndDateTime).ToList();
                    break;
            }

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
                if (_assignmentService.getActiveAssignmentByCourseID(_course.id) != null)
                {
                    foreach (AssignmentViewModel _assignment in _assignmentService.getActiveAssignmentByCourseID(_course.id))
                    {
                        _allAssignments.Add(_assignment);
                    }
                }
            }
            _allAssignments.Sort((x, y) => x.EndDateTime.CompareTo(y.EndDateTime));
            var _studentFrontPageViewModel = new StudentFrontPageViewModel() { Courses = _courses, Assignments = _allAssignments  };
            return View(_studentFrontPageViewModel);
        }
    }
}