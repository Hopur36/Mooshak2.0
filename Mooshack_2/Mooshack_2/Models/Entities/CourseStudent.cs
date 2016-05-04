using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Entities
{
    public class CourseStudent
    {
        public int idCourseStudent { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
    }
}