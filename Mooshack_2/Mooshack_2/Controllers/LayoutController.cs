using Microsoft.AspNet.Identity;
using Mooshack_2.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using Mooshack_2.Models.ViewModels;

namespace Mooshack_2.Controllers
{
    public class LayoutController : Controller
    {
        CourseService _courseService;

        public LayoutController()
        {
            _courseService = new CourseService( null );
        }

        [ChildActionOnly]
        [ActionName( "_listOfCourses" )]
        public ActionResult _activeCourseList()
        {
            if( User.IsInRole( "Administrator" ) )
            {
                var _allCourses = _courseService.getAllCourses();
                var _activeCourses = new List<CourseViewModel>();

                foreach( var _course in _allCourses )
                {
                    if( _course.Active == true )
                    {
                        _activeCourses.Add( _course );
                    }
                }
                _activeCourses.Sort( ( x, y ) => x.Name.CompareTo( y.Name ) );

                return PartialView( "_listOfCourses", _activeCourses );
            }
            else if( User.IsInRole( "Teacher" ) )
            {
                var _courses = _courseService.getAllActiveCoursesByTeacherID( User.Identity.GetUserId() );

                return PartialView( "_listOfCourses", _courses );
            }
            else
            {
                var _courses = _courseService.getAllActiveCoursesByStudentID( User.Identity.GetUserId() );

                return PartialView( "_listOfCourses", _courses );
            }
        }

        [ChildActionOnly]
        [ActionName( "_listOfUnactiveCourses" )]
        public ActionResult _unactiveCourseList()
        {
            /*
            if (User.IsInRole("Administrator"))
            {
                var _courses = _courseService.getAllInactiveCourses();
                _courses.Sort((x, y) => x.Name.CompareTo(y.Name));

                return PartialView("_listOfUnactiveCourses", _courses);
            }
            else */
            if( User.IsInRole( "Teacher" ) )
            {
                var _courses = _courseService.getAllInactiveCoursesByTeacherID( User.Identity.GetUserId() );

                return PartialView( "_listOfUnactiveCourses", _courses );
            }
            else
            {
                var _courses = _courseService.getAllInactiveCoursesByStudentID( User.Identity.GetUserId() );

                return PartialView( "_listOfUnactiveCourses", _courses );
            }
        }

        [ChildActionOnly]
        [ActionName( "AllCourses" )]
        public ActionResult _allCourses()
        {
            var _allCourses = _courseService.getAllCourses();

            return PartialView( "AllCourses", _allCourses );
        }
    }
}