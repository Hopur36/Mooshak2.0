using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class AdminCourseViewModel
    {
        [Required]
        public string Name { get; set; }
        public List<CourseViewModel> ActiveCourses { get; set; }
        public List<CourseViewModel> InactiveCourses { get; set; }
    }
}