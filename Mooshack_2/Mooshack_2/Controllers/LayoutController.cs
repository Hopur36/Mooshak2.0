using Microsoft.AspNet.Identity;
using Mooshack_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshack_2.Models.ViewModels;

namespace Mooshack_2.Controllers
{
    public class LayoutController : Controller
    {
        CourseService _courseService;

        public LayoutController()
        {
            _courseService = new CourseService(null);
        }

        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }


        
        [ChildActionOnly]
        [ActionName("_listOfCourses")]
        public ActionResult _activeCourseList()
        {

            if (User.IsInRole("Administrator"))
            {
                var _allCourses = _courseService.getAllCourses();
                var _activeCourses = new List<CourseViewModel>();

                foreach (var course in _allCourses)
                {
                    if(course.Active == true)
                    {
                        _activeCourses.Add(course);
                    }
                }
                _activeCourses.Sort((x, y) => x.Name.CompareTo(y.Name));

                return PartialView("_listOfCourses", _activeCourses);
            }
            else if (User.IsInRole("Teacher"))
            {
                var _courses = _courseService.getAllActiveCoursesByTeacherID(User.Identity.GetUserId());

                return PartialView("_listOfCourses", _courses);
            }
            else
            {
                var _courses = _courseService.getAllActiveCoursesByStudentID(User.Identity.GetUserId());

                return PartialView("_listOfCourses", _courses);
            }
        }

        
        
        [ChildActionOnly]
        [ActionName("_listOfUnactiveCourses")]
        public ActionResult _unactiveCourseList()
        {
            /*
            if (User.IsInRole("Administrator"))
            {
                var _courses = _courseService.getAllInactiveCourses();
                _courses.Sort((x, y) => x.Name.CompareTo(y.Name));

                return PartialView("_listOfUnactiveCourses", _courses);
            }
            else */if (User.IsInRole("Teacher"))
            {
                var _courses = _courseService.getAllInactiveCoursesByTeacherID(User.Identity.GetUserId());

                return PartialView("_listOfUnactiveCourses", _courses);
            }
            else
            {
                var _courses = _courseService.getAllInactiveCoursesByStudentID(User.Identity.GetUserId());

                return PartialView("_listOfUnactiveCourses", _courses);
            }
        }
        


    }

}