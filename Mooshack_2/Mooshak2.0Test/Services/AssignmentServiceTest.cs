using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mooshack_2.Services;
using Mooshak2._0Test;
//using Mooshack_2.Models.Entities;
using WebApplication1.Models.Entities;

namespace Mooshak2._0Test.Services
{
    [TestClass]
    public class AssignmentServiceTest
    {
        private AssignmentService _service;

        /// <summary>
        /// Instanciating a new MockDataBase and populating with data
        /// for running with Unit Tests.
        /// Each Unit Test initializes its variables with this data.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var mockdb = new MockDatabase();

            var a1 = new Assignment
            {
                id = 1,
                CourseID = 6,
                Title = "Reikningur"
            };
            mockdb.Assignments.Add(a1);

            var a2 = new Assignment
            {
                id = 2,
                CourseID = 5,
                Title = "Fibonacci"
            };
            mockdb.Assignments.Add(a2);

            var m1 = new Milestone
            {
                id = 1,
                AssignmentID = 2,
                Title = "Part1"
            };
            mockdb.Milestones.Add(m1);

            var m2 = new Milestone
            {
                id = 2,
                AssignmentID = 2,
                Title = "Part2"
            };
            mockdb.Milestones.Add(m2);

            var m3 = new Milestone
            {
                id = 3,
                AssignmentID = 3,
                Title = "Part1"
            };
            mockdb.Milestones.Add(m3);

            _service = new AssignmentService(mockdb);
        }

        /// <summary>
        /// Testing getAssignmentByCourseID with ID = 6.
        /// shall return 1 in count according to data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetAssignmentByCourseID6()
        {
            //Arrange:
            const int ID = 6;

            //Act:
            var result = _service.getAssignmentByCourseID(ID);

            //Assert:
            Assert.AreEqual(1, result.Count);
        }

        /// <summary>
        /// Testing getAssignmentByCourseID with ID = 6
        /// Asserting that the Title of the Assignment is correct
        /// given the data in MockDataBase.
        /// </summary>
        [TestMethod]
        public void TestGetAssignmentByCourseID()
        {
            //Arrange:
            const int ID = 6;

            //Act:
            var result = _service.getAssignmentByCourseID(ID);

            //Assert:
            Assert.AreEqual("Reikningur", result[0].Title);
        }

        /// <summary>
        /// Testing getAllAssignments function in AssignmentService.cs
        /// Returns the count of 2 given the data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetAllAssignments()
        {
            //Arrange:
            //Nothing to set up

            //Act:
            var result = _service.getAllAssignments();

            //Assert:
            Assert.AreEqual(2, result.Count);
        }

        /// <summary>
        /// Testing getAllMilestonesByAssignmentID from AssignmentService.cs
        /// Returns the count of 2 given the data in MockDataBase
        /// </summary>
        [TestMethod]
        public void TestGetAllMilestonesByAssignmentID()
        {
            //Arrange:
            const int _assignmentID = 2;

            //Act:
            var result = _service.getAllMilestonesByAssignmentID(_assignmentID);

            //Assert:
            Assert.AreEqual(2, result.Count);
        }

        /// <summary>
        /// Testing DeleteAssignment function
        /// Count all assignments before deletion and after.
        /// The latter count shall be one less than the before.
        /// </summary>
        [TestMethod]
        public void TestDeleteAssignment()
        {
            //Arrange:
            const int _assignmentID = 2;

            //Act:
            var assignmentsBefore = _service.getAllAssignments();
            var result = _service.deleteAssignment(_assignmentID);
            var assignmentsAfter = _service.getAllAssignments();

            //Assert:
            Assert.AreEqual(2, assignmentsBefore.Count);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1, assignmentsAfter.Count);
        }
    }
}
