using Mooshack_2.Models;
using Mooshack_2.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
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
        public CourseService( IMyDataContext context )
        {
            _dbContext = context ?? new ApplicationDbContext();
            _assignmentService = new AssignmentService( null );
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
            List<Course> _allCourses = (from _course in _dbContext.Courses
                                        select _course).ToList();

            //Create a new list of courseviewmodel
            List<CourseViewModel> _allCoursesViewModel = new List<CourseViewModel>();

            foreach( Course _c in _allCourses )
            {
                _allCoursesViewModel.Add( new CourseViewModel {Name = _c.Name, id = _c.ID, Active = _c.Active} );
            }

            return _allCoursesViewModel;
        }

        /// <summary>
        /// Get all inactive courses from database
        /// </summary>
        /// <returns></returns>
        public List<CourseViewModel> getAllInactiveCourses()
        {
            List<Course> _allCourses = (from _course in _dbContext.Courses
                                        select _course).ToList();

            //Create a new list of courseviewmodel
            List<CourseViewModel> _allCoursesViewModel = new List<CourseViewModel>();

            foreach( Course _c in _allCourses )
            {
                if( _c.Active != true )
                {
                    _allCoursesViewModel.Add( new CourseViewModel {Name = _c.Name} );
                }
            }

            return _allCoursesViewModel;
        }

        /// <summary>
        /// This function returns a CourseViewModel for a given
        /// course ID.
        /// </summary>
        /// <param name="cID"></param>
        /// <returns>CourseViewModel</returns>
        public CourseViewModel getCourseViewModelByID( int? cID )
        {
            //Get the coures with a given ID from database.
            Course _course = (from course in _dbContext.Courses
                              where course.ID == cID
                              select course).FirstOrDefault();

            //Make a CourseViewModel and fill in the attributes.
            if ( _course == null )
            {
                return null;
            }

            CourseViewModel _courseViewModel = new CourseViewModel {id = _course.ID, Name = _course.Name};

            return _courseViewModel;
        }

        /// <summary>
        /// This function returns a list of CourseViewModel with the courses that
        /// the teacher specified in "teacherID" teaches
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        public List<CourseViewModel> getAllCoursesByTeacherID( string teacherID )
        {
            //list of all entries in CourseTeachers that match the teacherID
            List<CourseTeacher> _allCourseTeachers = (from _courseteacher in _dbContext.CourseTeacher
                                                      where _courseteacher.TeacherID == teacherID
                                                      select _courseteacher).ToList();

            //Create a new list of courseviewmodel and only add courses that the teacher teaches
            List<CourseViewModel> _allCoursesWithTeacherViewModel = new List<CourseViewModel>();

            foreach ( CourseTeacher _ct in _allCourseTeachers )
            {
                foreach( Course _c in _dbContext.Courses )
                {
                    if( _c.ID == _ct.CourseID )
                    {
                        _allCoursesWithTeacherViewModel.Add( new CourseViewModel {id = _c.ID, Name = _c.Name} );
                    }
                }
            }

            return _allCoursesWithTeacherViewModel;
        }

        /// <summary>
        /// This function gets all active courses for a given teacher id.
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns>List of CourseViewModel</returns>
        public List<CourseViewModel> getAllActiveCoursesByTeacherID( string teacherID )
        {
            //list of all entries in CourseTeachers that match the teacherID
            List<CourseTeacher> _allCourseTeachers = (from _courseteacher in _dbContext.CourseTeacher
                                                      where _courseteacher.TeacherID == teacherID
                                                      select _courseteacher).ToList();

            //Create a new list of courseviewmodel and only add courses that the teacher teaches
            List<CourseViewModel> _allActiveCoursesWithTeacherViewModel = new List<CourseViewModel>();

            foreach ( CourseTeacher _ct in _allCourseTeachers )
            {
                foreach( Course _c in _dbContext.Courses )
                {
                    if( _c.ID == _ct.CourseID )
                    {
                        if( _c.Active == true )
                        {
                            _allActiveCoursesWithTeacherViewModel.Add( new CourseViewModel {id = _c.ID, Name = _c.Name} );
                        }
                    }
                }
            }

            return _allActiveCoursesWithTeacherViewModel;
        }

        /*This function returns a list of all unactive courses linked to each teacher*/

        public List<CourseViewModel> getAllInactiveCoursesByTeacherID( string teacherID )
        {
            //list of all entries in CourseTeachers that match the teacherID
            List<CourseTeacher> _allCourseTeachers = (from _courseteacher in _dbContext.CourseTeacher
                                                      where _courseteacher.TeacherID == teacherID
                                                      select _courseteacher).ToList();

            //Create a new list of courseviewmodel and only add courses that the teacher teaches
            List<CourseViewModel> _allInactiveCoursesWithTeacherViewModel = new List<CourseViewModel>();

            foreach( CourseTeacher _ct in _allCourseTeachers )
            {
                foreach( Course _c in _dbContext.Courses )
                {
                    if( _c.ID == _ct.CourseID )
                    {
                        if( _c.Active != true )
                        {
                            _allInactiveCoursesWithTeacherViewModel.Add( new CourseViewModel {id = _c.ID, Name = _c.Name} );
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
        public List<CourseViewModel> getAllCoursesByStudentID( string studentID )
        {
            //list of all entries in CourseStudents that match the studentID
            List<CourseStudent> _allCourseStudents = (from _coursestudent in _dbContext.CourseStudent
                                                      where _coursestudent.StudentID == studentID
                                                      select _coursestudent).ToList();

            //Create a new list of courseviewmodel and only add courses that the student is enrolled in
            List<CourseViewModel> _allCoursesWithStudentViewModel = new List<CourseViewModel>();

            foreach ( CourseStudent _cs in _allCourseStudents )
            {
                foreach( Course _c in _dbContext.Courses )
                {
                    if( _c.ID == _cs.CourseID )
                    {
                        _allCoursesWithStudentViewModel.Add( new CourseViewModel {id = _c.ID, Name = _c.Name} );
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
        public List<CourseViewModel> getAllActiveCoursesByStudentID( string studentID )
        {
            //list of all entries in CourseStudents that match the studentID
            List<CourseStudent> _allCourseStudents = (from _coursestudent in _dbContext.CourseStudent
                                                      where _coursestudent.StudentID == studentID
                                                      select _coursestudent).ToList();

            //Create a new list of courseviewmodel and only add active courses that the student is enrolled in
            List<CourseViewModel> _allActiveCoursesWithStudentViewModel = new List<CourseViewModel>();

            foreach ( CourseStudent _cs in _allCourseStudents )
            {
                foreach( Course _c in _dbContext.Courses )
                {
                    if( _c.ID == _cs.CourseID )
                    {
                        if( _c.Active == true )
                        {
                            _allActiveCoursesWithStudentViewModel.Add( new CourseViewModel {id = _c.ID, Name = _c.Name} );
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
        public List<CourseViewModel> getAllInactiveCoursesByStudentID( string studentID )
        {
            //list of all entries in CourseStudents that match the studentID
            List<CourseStudent> _allCourseStudents = (from _coursestudent in _dbContext.CourseStudent
                                                      where _coursestudent.StudentID == studentID
                                                      select _coursestudent).ToList();

            //Create a new list of courseviewmodel and only add active courses that the student is enrolled in
            List<CourseViewModel> _allInactiveCoursesWithStudentViewModel = new List<CourseViewModel>();

            foreach ( CourseStudent _cs in _allCourseStudents )
            {
                foreach( Course _c in _dbContext.Courses )
                {
                    if( _c.ID == _cs.CourseID )
                    {
                        if( _c.Active != true )
                        {
                            _allInactiveCoursesWithStudentViewModel.Add( new CourseViewModel {id = _c.ID, Name = _c.Name} );
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
        public Course getCourseByID( int courseID )
        {
            var _course = _dbContext.Courses.SingleOrDefault( x => x.ID == courseID );

            return _course;
        }

        /// <summary>
        /// This function creates a new course in the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool createCourse( AdminCourseViewModel model )
        {
            // Creates the Course object and fill in the attributes.
            Course _newCourse = new Course
            {
                Name = model.Name,
                Active = true
            };

            // Add that new Course to the database.
            _dbContext.Courses.Add( _newCourse );
            _dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// This function deletes a coures and all it's teacherlinks, 
        /// studentlinks, assignments and milestones.
        /// </summary>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public bool deleteCourse( int courseID )
        {
            // Get all the assignments linked to this coures.
            List<Assignment> _allAssignments = (from _assignment in _dbContext.Assignments
                                                where _assignment.CourseID == courseID
                                                select _assignment).ToList();
           
            // Delete all the assignments linked to this coures.
            foreach ( var _item in _allAssignments )
            {
                _assignmentService.deleteAssignment( _item.id );
            }

            // Get all the teachers linked to this coures.
            List<CourseTeacher> _allCourseTeachers = (from _ct in _dbContext.CourseTeacher
                                                      where _ct.CourseID == courseID
                                                      select _ct).ToList();
            // Delete the links for all teachers linked to this coures.
            foreach ( var _item in _allCourseTeachers )
            {
                removeTeacherFromCourse( _item.TeacherID, courseID );
            }

            // Get all the students linked to this coures.
            List<CourseStudent> _allCourseStudent = (from _cs in _dbContext.CourseStudent
                                                     where _cs.CourseID == courseID
                                                     select _cs).ToList();

            // Delete the links for all students linked to this coures.
            foreach ( var _item in _allCourseStudent )
            {
                removeStudentFromCourse( _item.StudentID, courseID );
            }

            // Get the course from the Courses table in the database.
            Course _deletedCourse = (from _course in _dbContext.Courses
                                     where _course.ID == courseID
                                     select _course).FirstOrDefault();
            // Delete that coures from the database.
            _dbContext.Courses.Remove( _deletedCourse );
            _dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// This function gets all the students in a given coures.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of UserViewModels</returns>
        public List<UserViewModel> getCourseStudents( int id )
        {
            // Get all the students from the database in a given course.
            List<CourseStudent> _studentsList = (from _course in _dbContext.CourseStudent
                                                 where _course.CourseID == id
                                                 select _course).ToList();
            // The list that will be returned.
            List<UserViewModel> _studentViewModelList = new List<UserViewModel>();

            // For each student in the coures,
            foreach( var _student in _studentsList )
            {
                /// get there info,
                ApplicationUser _userInfo = _dbContext.Users.SingleOrDefault( x => x.Id == _student.StudentID );
                // build a UserViewModel with that info and add it to the return list. 
                _studentViewModelList.Add( new UserViewModel
                {
                    Id = _userInfo.Id,
                    UserName = _userInfo.UserName,
                    Email = _userInfo.Email
                } );
            }

            // Sort the list by username.
            _studentViewModelList.Sort( ( x, y ) => x.UserName.CompareTo( y.UserName ) );

            return _studentViewModelList;
        }

        /// <summary>
        /// This function gets all the teachers in a given coures.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<UserViewModel> getCourseTeachers( int id )
        {
            // Get all the teachers from the database in a given course.
            List<CourseTeacher> _teachersList = (from _course in _dbContext.CourseTeacher
                                                 where _course.CourseID == id
                                                 select _course).ToList();
            // The list that will be returned.
            List<UserViewModel> _teacherViewModelList = new List<UserViewModel>();

            // For each teacher in the coures,
            foreach( var _teacher in _teachersList )
            {
                /// get there info,
                ApplicationUser _userInfo = _dbContext.Users.SingleOrDefault( x => x.Id == _teacher.TeacherID );
                // build a UserViewModel with that info and add it to the return list.
                _teacherViewModelList.Add( new UserViewModel
                {
                    Id = _userInfo.Id,
                    UserName = _userInfo.UserName,
                    Email = _userInfo.Email
                } );
            }

            // Sort the list by username.
            _teacherViewModelList.Sort( ( x, y ) => x.UserName.CompareTo( y.UserName ) );

            return _teacherViewModelList;
        }

        /// <summary>
        /// This function gets all the teachers in the database.
        /// </summary>
        /// <returns>List of UserViewModels</returns>
        public List<UserViewModel> getAllTeachers()
        {
            // Get all info for a Teacher tag.
            var _roleHash = _dbContext.Roles.SingleOrDefault( x => x.Name == "Teacher" );
            // Get all users with the "Theacher" hash string.
            var _allTeachers =
                _dbContext.Users.Where( x => x.Roles.Select( role => role.RoleId ).Contains( _roleHash.Id ) ).ToList();

            // The return list.
            List<UserViewModel> _teacherViewModelList = new List<UserViewModel>();

            // Make a UserViewModel for all the teachers and add it to the return list.
            foreach( var _teacher in _allTeachers )
            {
                _teacherViewModelList.Add( new UserViewModel
                {
                    Id = _teacher.Id,
                    UserName = _teacher.UserName,
                    Email = _teacher.Email
                } );
            }

            // Sort the list by username.
            _teacherViewModelList.Sort( ( x, y ) => x.UserName.CompareTo( y.UserName ) );

            return _teacherViewModelList;
        }

        /// <summary>
        /// This function gets all the students in the database.
        /// </summary>
        /// <returns></returns>
        public List<UserViewModel> getAllStudents()
        {
            // Get all info for a Student tag.
            var _roleHash = _dbContext.Roles.SingleOrDefault( x => x.Name == "Student" );
            // Get all users with the "Student" hash string.
            var _allStudents =
                _dbContext.Users.Where( x => x.Roles.Select( role => role.RoleId ).Contains( _roleHash.Id ) ).ToList();

            // The return list.
            List<UserViewModel> _studentViewModelList = new List<UserViewModel>();

            // Make a UserViewModel for all the students and add it to the return list.
            foreach( var _student in _allStudents )
            {
                _studentViewModelList.Add( new UserViewModel
                {
                    Id = _student.Id,
                    UserName = _student.UserName,
                    Email = _student.Email
                } );
            }

            // Sort the list by username.
            _studentViewModelList.Sort( ( x, y ) => x.UserName.CompareTo( y.UserName ) );

            return _studentViewModelList;
        }

        /// <summary>
        /// Returns a CourseViewModel with a course that has the same id as the course id of assignment
        /// </summary>
        /// <param name="assignmentID"></param>
        /// <returns>CourseViewModel</returns>
        public CourseViewModel getCourseViewModelByAssignmentID( int assignmentID )
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
            var _courseViewModel = new CourseViewModel
            {
                id = _course.ID,
                Name = _course.Name,
                Active = _course.Active
            };

            return _courseViewModel;
        }

        /// <summary>
        /// This function removes the student link to a course from the database.
        /// Cascading is done in the controller.
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public bool removeStudentFromCourse( string studentID, int courseID )
        {
            // Get the row in the CourseStudent table that is the link.
            CourseStudent _deletedCourseStudent = (from _course in _dbContext.CourseStudent
                                                   where _course.CourseID == courseID
                                                   where _course.StudentID == studentID
                                                   select _course).FirstOrDefault();
            // Delete that row.
            _dbContext.CourseStudent.Remove( _deletedCourseStudent );
            _dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// This function adds a student to a course by inserting 
        /// it in to the CourseStudent table in the database.
        /// </summary>
        /// <param name="studentName"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public bool addStudentToCourse( string studentName, int courseID )
        {
            // Get the students info from a given username.
            var _student = _dbContext.Users.FirstOrDefault( x => x.UserName == studentName );

            // Create a CourseStudent object with that info.
            CourseStudent _addCourseStudent = new CourseStudent
            {
                CourseID = courseID,
                StudentID = _student.Id
            };

            // Add it to the table.
            _dbContext.CourseStudent.Add( _addCourseStudent );
            _dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// This function removes the teachers link to a course from the database.
        /// Cascading is done in the controler.
        /// </summary>
        /// <param name="teacherID"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public bool removeTeacherFromCourse( string teacherID, int courseID )
        {
            // Get the row in the CourseTeachers table that is the link.
            CourseTeacher _deletedCourseTeacher = (from _course in _dbContext.CourseTeacher
                                                   where _course.CourseID == courseID
                                                   where _course.TeacherID == teacherID
                                                   select _course).FirstOrDefault();
            // Delete that row.
            _dbContext.CourseTeacher.Remove( _deletedCourseTeacher );
            _dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// This function adds a teacher to a course by inserting 
        /// it in to the CourseTeacher table in the database.
        /// </summary>
        /// <param name="teacherName"></param>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public bool addTeacherToCourse( string teacherName, int courseID )
        {
            // Get the teachers info from a given username.
            var _teacher = _dbContext.Users.FirstOrDefault( x => x.UserName == teacherName );

            // Create a CourseTeacher object with that info.
            CourseTeacher _addCourseTeacher = new CourseTeacher
            {
                CourseID = courseID,
                TeacherID = _teacher.Id
            };

            // Add it to the table.
            _dbContext.CourseTeacher.Add( _addCourseTeacher );
            _dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// This function returns a list of UserViewModels with all the 
        /// users in the database.
        /// </summary>
        /// <returns>List of UserViewModels with all the 
        /// users in the database</returns>
        public List<UserViewModel> getAllUsers()
        {
            // Get all the users for the database.
            var _allUsers = _dbContext.Users.ToList();

            // The return list.
            List<UserViewModel> _userViewModelList = new List<UserViewModel>();

            // Make a UserViewModel for all the users and add it to the return list.
            foreach( var _user in _allUsers )
            {
                _userViewModelList.Add( new UserViewModel {Id = _user.Id, UserName = _user.UserName, Email = _user.Email} );
            }

            // Sort the list by username.
            _userViewModelList.Sort( ( x, y ) => x.UserName.CompareTo( y.UserName ) );

            return _userViewModelList;
        }

        public bool isCourseActive( int courseID )
        {
            Course _result = getCourseByID( courseID );
            return _result.Active;
        }

        public void changeCourseActive( int courseID, bool active )
        {
            Course _course = getCourseByID( courseID );

            _course.Active = active;
            _dbContext.SaveChanges();
        }
    }
}