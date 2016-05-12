using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Models.Entities;
using Mooshack_2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshak2._0Test;

namespace Mooshak2._0Test.Services
{
    [TestClass]
    public class CourseServiceTest
    {
        private CourseService _service;

        /// <summary>
        /// Instanciating a new MockDataBase and populating with data
        /// for running with Unit Tests.
        /// Each Unit Test initializes its variables with this data.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var mockdb = new MockDatabase();

            var c1 = new Course
            {
                ID = 1,
                Name = "Vefforritun",
                Active = true
            };
            mockdb.Courses.Add(c1);

            var c2 = new Course
            {
                ID = 2,
                Name = "Vefforritun2",
                Active = false
            };
            mockdb.Courses.Add(c2);

            var c3 = new Course
            {
                ID = 3,
                Name = "Gagnaskipan",
                Active = true
            };
            mockdb.Courses.Add(c3);

            var c4 = new CourseTeacher
            {
                id = 1,
                TeacherID = "007",
                CourseID = 1
            };
            mockdb.CourseTeacher.Add(c4);

            var c5 = new CourseTeacher
            {
                id = 2,
                TeacherID = "999",
                CourseID = 2
            };
            mockdb.CourseTeacher.Add(c5);

            var c6 = new CourseTeacher
            {
                id = 3,
                TeacherID = "999",
                CourseID = 3
            };
            mockdb.CourseTeacher.Add(c6);

            _service = new CourseService(mockdb);
        }

        /// <summary>
        /// Testing the function getAllCourses in CourseService.
        /// Should return count of 3 based on data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetAllCourses()
        {
            //Arrange:
            //No Arrangement needed

            //Act:
            var result = _service.getAllCourses();

            //Assert:
            Assert.AreEqual(3, result.Count);
        }

        /// <summary>
        /// Testing the function getAllInactiveCourses in CourseService.
        /// Should return count of 1 based on data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetAllInactiveCourses()
        {
            //Arrange:
            //No Arrangement needed

            //Act:
            var result = _service.getAllInactiveCourses();

            //Assert:
            Assert.AreEqual(1, result.Count);
        }

        /// <summary>
        /// Testing the function getCoursesViewModelByID in CourseService.
        /// Should return the name Vefforritun based on data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetCourseViewModelById()
        {
            //Arrange:
            const int ID = 1;

            //Act:
            var result = _service.getCourseViewModelByID(ID);

            //Assert:
            Assert.AreEqual("Vefforritun", result.Name);
        }

        /// <summary>
        /// Testing the function getAllCoursesByTeacherID in CourseService.
        /// Should return count of 1 based on data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetAllCoursesByTeacherID()
        {
            //Arrange:
            const string teacherID = "007";

            //Act:
            var result = _service.getAllCoursesByTeacherID(teacherID);

            //Assert:
            Assert.AreEqual(1, result.Count);
        }

        /// <summary>
        /// Testing the function getAllInactiveCoursesByTeacherID in 
        /// CourseService.
        /// Should return count of 1 based on data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetAllInactiveCoursesByTeacherID()
        {
            //Arrange:
            const string teacherID = "999";

            //Act:
            var result = _service.getAllInactiveCoursesByTeacherID(teacherID);

            //Assert:
            Assert.AreEqual(1, result.Count);
        }
    }
}
