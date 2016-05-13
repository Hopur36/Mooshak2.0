using System.Collections.Generic;
using System.Web.Mvc;
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
            _courseService = new CourseService( null );
            _assignmentService = new AssignmentService( null );
        }

        public ActionResult index()
        {
            if( User.IsInRole( "Administrator" ) )
            {
                return RedirectToAction( "showAllUsers", "Account" );
            }
            else if( User.IsInRole( "Teacher" ) )
            {
                return RedirectToAction( "teacherFrontPage" );
            }
            else if( User.IsInRole( "Student" ) )
            {
                return RedirectToAction( "studentFrontPage" );
            }

            return RedirectToAction( "Login", "Account" );
        }

        public ActionResult about()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult teacherFrontPage()
        {
            var _courses = _courseService.getAllActiveCoursesByTeacherID( User.Identity.GetUserId() );
            List<AssignmentViewModel> _allAssignments = new List<AssignmentViewModel>();
            foreach( CourseViewModel _course in _courses )
            {
                if( _assignmentService.getActiveAssignmentByCourseID( _course.id ) != null )
                {
                    foreach(
                        AssignmentViewModel _assignment in
                            _assignmentService.getActiveAssignmentByCourseID( _course.id ) )
                    {
                        _allAssignments.Add( _assignment );
                    }
                }
            }
            _allAssignments.Sort( ( x, y ) => x.EndDateTime.CompareTo( y.EndDateTime ) );
            var _teacherFrontPageViewModel = new TeacherFrontPageViewModel()
            {
                Courses = _courses,
                Assignments = _allAssignments
            };

            return View( _teacherFrontPageViewModel );
        }

        [Authorize( Roles = "Administrator" )]
        public ActionResult adminFrontPage()
        {
            return View();
        }

        /// <summary>
        /// This function gets all active courses by StudentID and Assignments for those courses
        /// </summary>
        /// <returns>StudentFrontPageViewModel</returns>
        [Authorize( Roles = "Student" )]
        public ActionResult studentFrontPage()
        {
            var _courses = _courseService.getAllActiveCoursesByStudentID( User.Identity.GetUserId() );
            List<AssignmentViewModel> _allAssignments = new List<AssignmentViewModel>();
            foreach( CourseViewModel _course in _courses )
            {
                if( _assignmentService.getActiveAssignmentByCourseID( _course.id ) != null )
                {
                    foreach(
                        AssignmentViewModel _assignment in
                            _assignmentService.getActiveAssignmentByCourseID( _course.id ) )
                    {
                        _allAssignments.Add( _assignment );
                    }
                }
            }
            _allAssignments.Sort( ( x, y ) => x.EndDateTime.CompareTo( y.EndDateTime ) );
            var _studentFrontPageViewModel = new StudentFrontPageViewModel()
            {
                Courses = _courses,
                Assignments = _allAssignments
            };
            return View( _studentFrontPageViewModel );
        }
    }
}