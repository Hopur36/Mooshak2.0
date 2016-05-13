using System;

namespace WebApplication1.Models.Entities
{
    public class Submission
    {
        public int id{ get; set; }
        public string StudentID { get; set; }
        public int MilestoneID { get; set; }
        public DateTime DateTimeSubmitted { get; set; }
        public string ItemSubmittedPath { get; set; }
        public bool Accepted { get; set; }
    }
}