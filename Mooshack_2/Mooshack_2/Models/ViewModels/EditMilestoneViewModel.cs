using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class EditMilestoneViewModel : MilestoneViewModel
    {
        public int AssignmentID { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
    }
}