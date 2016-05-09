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
            var _courses = _courseService.getAllCourses();

            return PartialView("_listOfCourses", _courses);
        }


    }
}