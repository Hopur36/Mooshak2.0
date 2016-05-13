using System;

namespace WebApplication1.Models.Entities
{
    public class Assignment
    {
        public int id { get; set; }
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}