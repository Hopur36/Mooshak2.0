﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Entities
{
    public class CourseTeacher
    {
        public int id { get; set; }
        public string TeacherID { get; set; }
        public int CourseID { get; set; }
    }
}