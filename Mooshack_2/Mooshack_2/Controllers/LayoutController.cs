﻿using Microsoft.AspNet.Identity;
using Mooshack_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshack_2.Controllers
{
    public class LayoutController : Controller
    {
        CourseService _courseService;

        public LayoutController()
        {
            _courseService = new CourseService();
        }

        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        [ActionName("_listOfCourses")]
        public ActionResult _courseList()
        {

            if (User.IsInRole("Administrator"))
            {
                var _courses = _courseService.getAllCourses();

                return PartialView("_listOfCourses", _courses);
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
    }

}