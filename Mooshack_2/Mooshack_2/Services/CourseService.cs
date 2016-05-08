﻿using Mooshack_2.Models;
using Mooshack_2.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.Entities;

namespace Mooshack_2.Services
{
    public class CourseService
    {
        private ApplicationDbContext _dbContext;

        public CourseService()
        {
            _dbContext = new ApplicationDbContext();
        }

        public List<Course> getAllCourses()
        {
            var _allCourses = _dbContext.Courses.ToList();
            return _allCourses;
        }

        /// <summary>
        /// This function returns a list of CourseViewModel with the courses that
        /// the teacher specified in "teacherID" teaches
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        public List<CourseViewModel> getAllCoursesByTeacherID(string teacherID)
        {
            //list of all entries in CourseTeachers that match the teacherID
            List<CourseTeacher> _allCourseTeachers = (from courseteacher in _dbContext.CourseTeacher
                                                      where courseteacher.TeacherID == teacherID
                                                      select courseteacher).ToList();

            //Create a new list of courseviewmodel and only add courses that the teacher teaches
            List<CourseViewModel> _allCoursesWithTeacherViewModel = new List<CourseViewModel>();
            foreach(CourseTeacher ct in _allCourseTeachers)
            {
                foreach(Course c in _dbContext.Courses)
                {
                    if(c.id == ct.CourseID)
                    {
                       _allCoursesWithTeacherViewModel.Add(new CourseViewModel { id = c.id, Name = c.Name });
                   }
                }
            }


            return _allCoursesWithTeacherViewModel;
        }

        /// <summary>
        /// This function returns a list of CourseViewModel with the courses that
        /// the student specified in "studentID" is enrolled in
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns>List<CourseViewModel></returns>
        public List<CourseViewModel> getAllCoursesByStudentID(string studentID)
        {
            //list of all entries in CourseStudents that match the studentID
            List<CourseStudent> _allCourseStudents = (from coursestudent in _dbContext.CourseStudent
                                                      where coursestudent.StudentID == studentID
                                                      select coursestudent).ToList();

            //Create a new list of courseviewmodel and only add courses that the student is enrolled in
            List<CourseViewModel> _allCoursesWithStudentViewModel = new List<CourseViewModel>();
            foreach (CourseStudent cs in _allCourseStudents)
            {
                foreach (Course c in _dbContext.Courses)
                {
                    if (c.id == cs.CourseID)
                    {
                        _allCoursesWithStudentViewModel.Add(new CourseViewModel { id = c.id, Name = c.Name });
                    }
                }
            }


            return _allCoursesWithStudentViewModel;

        }

        /// <summary>
        /// this function returns a single course with the id that matches "CourseID"
        /// </summary>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        public Course getCourseByID(int courseID)
        {
            var _course = _dbContext.Courses.SingleOrDefault(x => x.id == courseID);
            return _course;
        }



    }
}