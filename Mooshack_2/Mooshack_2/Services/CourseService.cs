using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        AssignmentService _assignmentService;

        /// <summary>
        /// Constructor for CourseService,
        /// if argument is null then new ApplicationDbContext() is used
        /// </summary>
        /// <param name="context"></param>
        public CourseService(IMyDataContext context)
        {
            _dbContext = context ?? new ApplicationDbContext();
            _assignmentService = new AssignmentService(null);
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
            List<Assignment> _allAssignments = (from assignment in _dbContext.Assignments
                                                where assignment.CourseID == courseID
                                                select assignment).ToList();
            foreach (var item in _allAssignments)
            {
                _assignmentService.DeleteAssignment(item.id);
            }

            List<CourseTeacher> _allCourseTeachers = (from ct in _dbContext.CourseTeacher
                                                where ct.CourseID == courseID
                                                select ct).ToList();
            foreach (var item in _allCourseTeachers)
            {
                removeTeacherFromCourse(item.TeacherID, courseID);
            }

            List<CourseStudent> _allCourseStudent = (from cs in _dbContext.CourseStudent
                                                      where cs.CourseID == courseID
                                                      select cs).ToList();
            foreach (var item in _allCourseStudent)
            {
                removeStudentFromCourse(item.StudentID, courseID);
            }

            Course _deletedCourse = (from course in _dbContext.Courses
                                             where course.ID == courseID
                                             select course).FirstOrDefault();
            _dbContext.Courses.Remove(_deletedCourse);
            _dbContext.SaveChanges();

            return true;
        }

        public List<UserViewModel> getCourseStudents(int id)
        {
            List<CourseStudent> _studentsList = (from course in _dbContext.CourseStudent
                                            where course.CourseID == id
                                            select course).ToList();

            List<UserViewModel> _studentViewModelList = new List<UserViewModel>();

            foreach (var student in _studentsList)
            {
                ApplicationUser _userInfo = _dbContext.Users.SingleOrDefault(x => x.Id == student.StudentID);
                _studentViewModelList.Add(new UserViewModel { Id = _userInfo.Id, UserName = _userInfo.UserName, Email = _userInfo.Email });
            }

            _studentViewModelList.Sort((x, y) => x.UserName.CompareTo(y.UserName));

            return _studentViewModelList;
        }

        public List<UserViewModel> getCourseTeachers(int id)
        {
            List<CourseTeacher> _teachersList = (from course in _dbContext.CourseTeacher
                                                 where course.CourseID == id
                                                 select course).ToList();

            List<UserViewModel> _teacherViewModelList = new List<UserViewModel>();

            foreach (var teacher in _teachersList)
            {
                ApplicationUser _userInfo = _dbContext.Users.SingleOrDefault(x => x.Id == teacher.TeacherID);
                _teacherViewModelList.Add(new UserViewModel { Id = _userInfo.Id, UserName = _userInfo.UserName, Email = _userInfo.Email });
            }

            _teacherViewModelList.Sort((x, y) => x.UserName.CompareTo(y.UserName));

            return _teacherViewModelList;
        }

        public List<UserViewModel> getAllTeachers()
        {
            var _roleHash = _dbContext.Roles.SingleOrDefault(x => x.Name == "Teacher");
            var _allTeachers = _dbContext.Users.Where(x => x.Roles.Select(role => role.RoleId).Contains(_roleHash.Id)).ToList();

            List<UserViewModel> _teacherViewModelList = new List<UserViewModel>();

            foreach (var teacher in _allTeachers)
            {
                _teacherViewModelList.Add(new UserViewModel { Id = teacher.Id, UserName = teacher.UserName, Email = teacher.Email });
            }

            _teacherViewModelList.Sort((x, y) => x.UserName.CompareTo(y.UserName));

            return _teacherViewModelList;
        }

        public List<UserViewModel> getAllStudents()
        {
            var _roleHash = _dbContext.Roles.SingleOrDefault(x => x.Name == "Student");
            var _allStudents = _dbContext.Users.Where(x => x.Roles.Select(role => role.RoleId).Contains(_roleHash.Id)).ToList();

            List<UserViewModel> _studentViewModelList = new List<UserViewModel>();

            foreach (var student in _allStudents)
            {
                _studentViewModelList.Add(new UserViewModel { Id = student.Id, UserName = student.UserName, Email = student.Email });
            }

            _studentViewModelList.Sort((x, y) => x.UserName.CompareTo(y.UserName));
            return _studentViewModelList;
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


        
        public bool removeStudentFromCourse(string studentID, int courseID)
        {
            CourseStudent _deletedCourseStudent = (from course in _dbContext.CourseStudent
                                     where course.CourseID == courseID
                                     where course.StudentID == studentID
                                     select course).FirstOrDefault();
            _dbContext.CourseStudent.Remove(_deletedCourseStudent);
            _dbContext.SaveChanges();

            return true;
        }

        public bool addStudentToCourse(string studentName, int courseID)
        {
            var _student = _dbContext.Users.FirstOrDefault(x => x.UserName == studentName);

            CourseStudent _addCourseStudent = new CourseStudent
            {
                CourseID = courseID,
                StudentID = _student.Id
            };
            _dbContext.CourseStudent.Add(_addCourseStudent);
            _dbContext.SaveChanges();

            return true;
        }

        public bool removeTeacherFromCourse(string teacherID, int courseID)
        {
            CourseTeacher _deletedCourseTeacher = (from course in _dbContext.CourseTeacher
                                                   where course.CourseID == courseID
                                                   where course.TeacherID == teacherID
                                                   select course).FirstOrDefault();
            _dbContext.CourseTeacher.Remove(_deletedCourseTeacher);
            _dbContext.SaveChanges();

            return true;
        }

        public bool addTeacherToCourse(string teacherName, int courseID)
        {
            var _teacher = _dbContext.Users.FirstOrDefault(x => x.UserName == teacherName);

            CourseTeacher _addCourseTeacher = new CourseTeacher
            {
                CourseID = courseID,
                TeacherID = _teacher.Id
            };
            //var _test = _dbContext.CourseTeacher.Contains(_addCourseTeacher);
            _dbContext.CourseTeacher.Add(_addCourseTeacher);
            _dbContext.SaveChanges();

            return true;
        }

        public List<UserViewModel> getAllUsers()
        {
            var _allUsers = _dbContext.Users.ToList();

            List<UserViewModel> _userViewModelList = new List<UserViewModel>();

            foreach (var user in _allUsers)
            {
                _userViewModelList.Add(new UserViewModel { Id = user.Id, UserName = user.UserName, Email = user.Email });
            }

            _userViewModelList.Sort((x, y) => x.UserName.CompareTo(y.UserName));
            return _userViewModelList;
        }

        public bool isCourseActive(int courseID)
        {
            Course _result = getCourseByID(courseID);
            return _result.Active;

        }

        public void changeCourseActive(int courseID, bool active)
        {
            Course _course = getCourseByID(courseID);

            _course.Active = active;
            _dbContext.SaveChanges();
        }

    }
}