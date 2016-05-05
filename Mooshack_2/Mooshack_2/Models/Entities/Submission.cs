using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Entities
{
    public class Submission
    {
        public int id{ get; set; }
        public string StudentID { get; set; }
        public int MilestoneID { get; set; }
    }
}