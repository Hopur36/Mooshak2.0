using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class CourseUsersViewModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public List<UserViewModel> Teachers { get; set; }
        public List<UserViewModel> Students { get; set; }
        public List<UserViewModel> TeachersRest { get; set; }
        public List<UserViewModel> StudentsRest { get; set; }
    }
}