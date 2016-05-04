using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Entities
{
    public class CourseTeacher
    {
        public int idCourseTeacher { get; set; }
        public int TeacherID { get; set; }
        public int CourseID { get; set; }
    }
}