using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Entities
{
    public class Milestone
    {
        public int idMilestones { get; set; }
        public int AssignmentID { get; set; }
        public string Title { get; set; }
        public int Description { get; set; }
        public int Weight { get; set; }
    }
}