﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class TeacherAssignmentViewModel
    {
        public string CourseName { get; set; }
        public List<AssignmentViewModel> Assignments { get; set; }
    }
}