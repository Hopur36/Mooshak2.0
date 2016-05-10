using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class StudentAssignmentViewModel
    {
        public string CourseName { get; set; }
        public int CourseID { get; set; }
        public List<AssignmentViewModel> Assignments { get; set; }

    }
}