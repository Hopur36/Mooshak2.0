﻿using Microsoft.AspNet.Identity;
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
            _courseService = new CourseService();
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

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult AdminCoursePage()
        {
            AdminCourseViewModel _myCourse = new AdminCourseViewModel();
            _myCourse.Courses = _courseService.getAllCourses();
            _myCourse.Courses.Sort((x,y) => x.Name.CompareTo(y.Name));

            return View(_myCourse);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AdminCoursePage(AdminCourseViewModel model)
        {
            var _course = _courseService.createCourse(model);
            AdminCourseViewModel _myCourse = new AdminCourseViewModel();
            _myCourse.Courses = _courseService.getAllCourses();

            return RedirectToAction("AdminCoursePage", "Course");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult deleteCourse(int courseID)
        {
            _courseService.deleteCourse(courseID);
            return RedirectToAction("AdminCoursePage", "Course");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public ActionResult AdminCourseUsersPage(string Name, int id)
        {
            CourseUsersViewModel _myViewModel = new CourseUsersViewModel();
            _myViewModel.id = id;
            _myViewModel.Name = Name;
            _myViewModel.Teachers = _courseService.getCourseTeachers(id);
            _myViewModel.Students = _courseService.getCourseStudents(id);
            _myViewModel.AllTeachers = _courseService.getAllTeachers();
            _myViewModel.AllStudents = _courseService.getAllStudents();

            return View(_myViewModel);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult removeStudentFromCourse(string studentID, int courseID, string courseName)
        {
            _courseService.removeStudentFromCourse(studentID, courseID);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = courseName, id = courseID });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddStudentToCourse(CourseUsersViewModel model, FormCollection form)
        {
            var studentName = form["studentSelect"].ToString();
            _courseService.addStudentToCourse(studentName, model.id);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = model.Name, id = model.id });
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult removeTeacherFromCourse(string teacherID, int courseID, string courseName)
        {
            _courseService.removeTeacherFromCourse(teacherID, courseID);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = courseName, id = courseID });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddTeacherToCourse(CourseUsersViewModel model, FormCollection form)
        {
            var teacherName = form["teacherSelect"].ToString();
            _courseService.addTeacherToCourse(teacherName, model.id);
            return RedirectToAction("AdminCourseUsersPage", "Course", new { Name = model.Name, id = model.id });
        }

    }
}