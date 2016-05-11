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

        [TestInitialize]
        public void Initialize()
        {
            var mockdb = new MockDatabase();
            var c1 = new Assignment
            {
                CourseID = 6,
                Title = "Reikningur"
            };
            mockdb.Assignments.Add(c1);
            var c2 = new Assignment
            {
                CourseID = 5,
                Title = "Plehhh"
            };
            mockdb.Assignments.Add(c2);

            _service = new AssignmentService(mockdb);
        }
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
    }
}
