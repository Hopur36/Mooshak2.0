using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Entities
{
    public class Course
    {
        public int id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}