using Mooshack_2.Models;
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

        public Course getCourseByID(int CourseID)
        {
            var _course = _dbContext.Courses.SingleOrDefault(x => x.id == CourseID);
            return _course;
        }


    }
}