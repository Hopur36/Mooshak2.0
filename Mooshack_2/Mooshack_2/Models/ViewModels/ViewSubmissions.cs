﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Models.ViewModels
{
    public class ViewSubmissions
    {
        public int MilestoneID { get; set; }
        public List<StudentSubmissionViewModel> Submissions { get; set; }
    }
}