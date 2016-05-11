using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class StudentSubmissionViewModel
    {
        public int id { get; set; }
        public string StudentID { get; set; }
        public int MilestoneID { get; set; }
        public string MilestoneTitle { get; set; }
        public string MilestoneDescription { get; set; }
        public DateTime DateTimeSubmitted { get; set; }

        [Required]
        [Display(Name = "File")]
        public string FilePath { get; set; }
    }
}