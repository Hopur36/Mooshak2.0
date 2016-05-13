using Microsoft.AspNet.Identity;
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
    public class CourseController : Controller
    {
        CourseService _courseService;
        
        public CourseController()
        {
            _courseService = new CourseService(null);
        }

        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This function gets a list of courseviewmodel returns a view
        /// </summary>
        /// <returns>View</returns>
        [Authorize(Roles ="Teacher")]
        public ActionResult TeacherCoursePage()
        {
            var _courses = _courseService.getAllCoursesByTeacherID(User.Identity.GetUserId());

            return View(_courses);
        }

        /// <summary>
        /// This is the GET action when AdminCoursePage is loaded. It gets all 
        /// courses from the CourseService and sorts them by name and returns it to the view.
        /// </summary>
        /// <returns>Returns a view with a AdminCourseViewModel</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult AdminCoursePage()
        {
            AdminCourseViewModel _myCourse = new AdminCourseViewModel();
            _myCourse.ActiveCourses = new List<CourseViewModel>();
            _myCourse.InactiveCourses = new List<CourseViewModel>();
            List<CourseViewModel> _allCourses = _courseService.getAllCourses();
            foreach (var course in _allCourses)
            {
                if (course.Active)
                {
                    _myCourse.ActiveCourses.Add(course);
                }
                else
                {
                    _myCourse.InactiveCourses.Add(course);
                }
            }

            _myCourse.ActiveCourses.Sort((x,y) => x.Name.CompareTo(y.Name));
            _myCourse.InactiveCourses.Sort((x, y) => x.Name.CompareTo(y.Name));

            return View(_myCourse);
        }

        /// <summary>
        /// This is the POST action when AdminCoursePage posts a form.
        /// Its starts of by creating the new course by calling createCourse
        /// in _courseService and then going back to AdminCoursePage.
        /// </summary>
        /// <param name="model"></param>
        /// <returns> Goes back to AdminCoursePage </returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AdminCoursePage(AdminCourseViewModel model)
        {
            var _course = _courseService.createCourse(model);
            return RedirectToAction("AdminCoursePage", "Course");
        }

        /// <summary>
        /// Deletes the course with the given id by calling
        /// DeleteCourse in _courseService.
        /// </summary>
        /// <param name="courseID"></param>
        /// <returns> Goes back to AdminCoursePage </returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult deleteCourse(int courseID)
        {
            _courseService.deleteCourse(courseID);
            return RedirectToAction("AdminCoursePage", "Course");
        }

        /// <summary>
        /// This is the GET action for AdminCourseUserPage. What it does is fill the 
        /// viewModel that is used on the site. It gets all the users liked to a 
        /// given course ID and then puts the rest in a seperet list.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="id"></param>
        /// <param name="Active"></param>
        /// <returns>A view with a CourseUserViewModel</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult AdminCourseUsersPage(string Name, int id, bool Active)
        {
            CourseUsersViewModel _myViewModel = new CourseUsersViewModel();
            _myViewModel.id = id;
            _myViewModel.Name = Name;
            _myViewModel.Active = Active;
            _myViewModel.Teachers = _courseService.getCourseTeachers(id);
            _myViewModel.Students = _courseService.getCourseStudents(id);
            _myViewModel.TeachersRest = new List<UserViewModel>();
            _myViewModel.StudentsRest = new List<UserViewModel>();

            List<UserViewModel> _allTeachers = _courseService.getAllTeachers();
            List<UserViewModel> _allStudents = _courseService.getAllStudents();

            foreach (var item in _allTeachers)
            {
                if (_myViewModel.Teachers.SingleOrDefault(x => x.UserName == item.UserName ) == null)
                {
                    _myViewModel.TeachersRest.Add(item);
                }
            }

            foreach (var item in _allStudents)
            {
                if (_myViewModel.Students.SingleOrDefault(x => x.UserName == item.UserName) == null)
                {
                    _myViewModel.StudentsRest.Add(item);
                }
            }
            return View(_myViewModel);
        }

        /// <summary>
        /// This removes a Student from a Course by calling _courseService with 
        /// a student and course ID.
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="courseID"></param>
        /// <param name="courseName"></param>
        /// <param name="Active"></param>
        /// <returns> Goes back to AdminCourseUsersPage</returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult removeStudentFromCourse(string studentID, int courseID, string courseName,bool Active)
        {
            _courseService.removeStudentFromCourse(studentID, courseID);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = courseName, id = courseID,Active = Active });
        }

        /// <summary>
        /// This uses a FormCollection to get what student name was selected in the
        /// dropdown when the form was sent. Then adds that student name to the course by
        /// calling addStudentToCourse in _courseService.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns> Goes back to AdminCourseUsersPage</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddStudentToCourse(CourseUsersViewModel model, FormCollection form)
        {
            var studentName = form["studentSelect"].ToString();
            _courseService.addStudentToCourse(studentName, model.id);
            model.Active = _courseService.isCourseActive(model.id);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = model.Name, id = model.id, Active = model.Active });
        }

        /// <summary>
        /// Removes teacher form course by calling removeTeacherFormCourse in _courseService.
        /// </summary>
        /// <param name="teacherID"></param>
        /// <param name="courseID"></param>
        /// <param name="courseName"></param>
        /// <param name="Active"></param>
        /// <returns> Goes back to AdminCourseUserPage</returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult removeTeacherFromCourse(string teacherID, int courseID, string courseName, bool Active)
        {
            _courseService.removeTeacherFromCourse(teacherID, courseID);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = courseName, id = courseID, Active = Active});
        }

        /// <summary>
        /// This uses a FormCollection to get what teacher name was selected in the
        /// dropdown when the form was sent. Then adds that teacher name to the course by
        /// calling addTeacherToCourse in _courseService.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddTeacherToCourse(CourseUsersViewModel model, FormCollection form)
        {
            var teacherName = form["teacherSelect"].ToString();
            _courseService.addTeacherToCourse(teacherName, model.id);
            model.Active = _courseService.isCourseActive(model.id);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = model.Name, id = model.id, Active = model.Active });
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult courseActivateOrDeactivate(string Name, int id, bool Active)
        {
            _courseService.changeCourseActive(id, Active);

            Active = _courseService.isCourseActive(id);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = Name, id = id, Active = Active});
        }

    }
}