using Mooshack_2.Models;
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
        /// <summary>
        /// Variable can not be changed, unless it is in a constructor
        /// </summary>
        private readonly IMyDataContext _dbContext;

        /// <summary>
        /// Constructor for CourseService,
        /// if argument is null then new ApplicationDbContext() is used
        /// </summary>
        /// <param name="context"></param>
        public CourseService(IMyDataContext context)
        {
            _dbContext = context ?? new ApplicationDbContext();
        }

        /*private ApplicationDbContext _dbContext;

        public CourseService()
        {
            _dbContext = new ApplicationDbContext();
        }*/

        /// <summary>
        /// Get all courses from database
        /// </summary>
        /// <returns></returns>
        public List<CourseViewModel> getAllCourses()
        {
            List<Course> _allCourses = (from course in _dbContext.Courses
                                        select course).ToList();

            //Create a new list of courseviewmodel
            List<CourseViewModel> _allCoursesViewModel = new List<CourseViewModel>();

            foreach (Course c in _allCourses)
            {              
                _allCoursesViewModel.Add(new CourseViewModel { Name = c.Name, id = c.ID, Active = c.Active });
            }
            return _allCoursesViewModel;
        }

        /// <summary>
        /// Get all inactive courses from database
        /// </summary>
        /// <returns></returns>
        public List<CourseViewModel> getAllInactiveCourses()
        {
            List<Course> _allCourses = (from course in _dbContext.Courses
                                        select course).ToList();

            //Create a new list of courseviewmodel
            List<CourseViewModel> _allCoursesViewModel = new List<CourseViewModel>();

            foreach (Course c in _allCourses)
            {
                if (c.Active != true)
                {
                    _allCoursesViewModel.Add(new CourseViewModel { Name = c.Name });
                }
            }
            return _allCoursesViewModel;
        }


        public CourseViewModel getCourseViewModelByID(int? cID)
        {
            Course _course = (from course in _dbContext.Courses
                              where course.ID == cID
                              select course).FirstOrDefault();
            CourseViewModel _courseViewModel = new CourseViewModel { id = _course.ID, Name = _course.Name };
            return _courseViewModel;
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
            foreach (CourseTeacher ct in _allCourseTeachers)
            {
                foreach (Course c in _dbContext.Courses)
                {
                    if (c.ID == ct.CourseID)
                    {
                        _allCoursesWithTeacherViewModel.Add(new CourseViewModel { id = c.ID, Name = c.Name });
                    }
                }
            }
            return _allCoursesWithTeacherViewModel;
        }
        public List<CourseViewModel> getAllActiveCoursesByTeacherID(string teacherID)
        {
            //list of all entries in CourseTeachers that match the teacherID
            List<CourseTeacher> _allCourseTeachers = (from courseteacher in _dbContext.CourseTeacher
                                                      where courseteacher.TeacherID == teacherID
                                                      select courseteacher).ToList();

            //Create a new list of courseviewmodel and only add courses that the teacher teaches
            List<CourseViewModel> _allActiveCoursesWithTeacherViewModel = new List<CourseViewModel>();
            foreach (CourseTeacher ct in _allCourseTeachers)
            {
                foreach (Course c in _dbContext.Courses)
                {
                    if (c.ID == ct.CourseID)
                    {
                        if(c.Active == true) 
                        {
                            _allActiveCoursesWithTeacherViewModel.Add(new CourseViewModel { id = c.ID, Name = c.Name });
                        }
                    }
                }
            }
            return _allActiveCoursesWithTeacherViewModel;
        }

        /*This function returns a list of all unactive courses linked to each teacher*/
        public List<CourseViewModel> getAllInactiveCoursesByTeacherID(string teacherID)
        {
            //list of all entries in CourseTeachers that match the teacherID
            List<CourseTeacher> _allCourseTeachers = (from courseteacher in _dbContext.CourseTeacher
                                                      where courseteacher.TeacherID == teacherID
                                                      select courseteacher).ToList();

            //Create a new list of courseviewmodel and only add courses that the teacher teaches
            List<CourseViewModel> _allInactiveCoursesWithTeacherViewModel = new List<CourseViewModel>();
            foreach (CourseTeacher ct in _allCourseTeachers)
            {
                foreach (Course c in _dbContext.Courses)
                {
                    if (c.ID == ct.CourseID)
                    {
                        if (c.Active != true)
                        {
                            _allInactiveCoursesWithTeacherViewModel.Add(new CourseViewModel { id = c.ID, Name = c.Name });
                        }
                    }
                }
            }
            return _allInactiveCoursesWithTeacherViewModel;
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
                    if (c.ID == cs.CourseID)
                    {
                        _allCoursesWithStudentViewModel.Add(new CourseViewModel { id = c.ID, Name = c.Name });
                    }
                }
            }
            return _allCoursesWithStudentViewModel;
        }

        /// <summary>
        /// This function returns a list of CourseViewModel with the active courses that
        /// the student specified in "studentID" is enrolled in
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns>List<CourseViewModel></returns>
        public List<CourseViewModel> getAllActiveCoursesByStudentID(string studentID)
        {
            //list of all entries in CourseStudents that match the studentID
            List<CourseStudent> _allCourseStudents = (from coursestudent in _dbContext.CourseStudent
                                                      where coursestudent.StudentID == studentID
                                                      select coursestudent).ToList();

            //Create a new list of courseviewmodel and only add active courses that the student is enrolled in
            List<CourseViewModel> _allActiveCoursesWithStudentViewModel = new List<CourseViewModel>();
            foreach (CourseStudent cs in _allCourseStudents)
            {
                foreach (Course c in _dbContext.Courses)
                {
                    if (c.ID == cs.CourseID)
                    {
                        if (c.Active == true)
                        {
                            _allActiveCoursesWithStudentViewModel.Add(new CourseViewModel { id = c.ID, Name = c.Name });
                        }
                    }
                }
            }
            return _allActiveCoursesWithStudentViewModel;
        }

        /// <summary>
        /// This function returns a list of CourseViewModel with the inactive courses that
        /// the student specified in "studentID" is enrolled in
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns>List<CourseViewModel></returns>
        public List<CourseViewModel> getAllInactiveCoursesByStudentID(string studentID)
        {
            //list of all entries in CourseStudents that match the studentID
            List<CourseStudent> _allCourseStudents = (from coursestudent in _dbContext.CourseStudent
                                                      where coursestudent.StudentID == studentID
                                                      select coursestudent).ToList();

            //Create a new list of courseviewmodel and only add active courses that the student is enrolled in
            List<CourseViewModel> _allInactiveCoursesWithStudentViewModel = new List<CourseViewModel>();
            foreach (CourseStudent cs in _allCourseStudents)
            {
                foreach (Course c in _dbContext.Courses)
                {
                    if (c.ID == cs.CourseID)
                    {
                        if (c.Active != true)
                        {
                            _allInactiveCoursesWithStudentViewModel.Add(new CourseViewModel { id = c.ID, Name = c.Name });
                        }
                    }
                }
            }
            return _allInactiveCoursesWithStudentViewModel;
        }

        /// <summary>
        /// this function returns a single course with the id that matches "CourseID"
        /// </summary>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        public Course getCourseByID(int courseID)
        {
            var _course = _dbContext.Courses.SingleOrDefault(x => x.ID == courseID);
            return _course;
        }

        public bool createCourse(AdminCourseViewModel model)
        {
            Course _newCourse = new Course
            {
                Name = model.Name,
                Active = true
            };

            _dbContext.Courses.Add(_newCourse);
            _dbContext.SaveChanges();
            return true;
        }

        public bool deleteCourse(int courseID)
        {
            Course _deletedCourse = (from course in _dbContext.Courses
                                             where course.ID == courseID
                                             select course).FirstOrDefault();
            _dbContext.Courses.Remove(_deletedCourse);
            _dbContext.SaveChanges();

            return true;
        }


        /// <summary>
        /// Returns a CourseViewModel with a course that has the same id as the course id of assignment
        /// </summary>
        /// <param name="assignmentID"></param>
        /// <returns>CourseViewModel</returns>
        public CourseViewModel getCourseViewModelByAssignmentID(int assignmentID)
        {
            //Find the assignment with the id "assignmentID"
            var _assignment = (from assignment in _dbContext.Assignments
                               where assignment.id == assignmentID
                               select assignment).FirstOrDefault();

            //Find the course with the same course id as the assignment "assignmentID"
            var _course = (from course in _dbContext.Courses
                           where course.ID == _assignment.CourseID
                           select course).FirstOrDefault();

            //create a new CourseViewModel
            var courseViewModel = new CourseViewModel { id = _course.ID,
                                                        Name = _course.Name,
                                                        Active = _course.Active};

            return courseViewModel;
        }




    }
}