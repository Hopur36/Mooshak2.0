using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Entities
{
    public class Milestone
    {
        public int id { get; set; }
        public int AssignmentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
    }
}