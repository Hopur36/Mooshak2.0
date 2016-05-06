using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class AssignmentViewModel
    {
        public int id { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}