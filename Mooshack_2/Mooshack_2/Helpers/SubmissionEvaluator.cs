using Mooshack_2.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Helpers
{
    public class SubmissionEvaluator
    {
        private StudentSubmissionViewModel _submission { get; set; }
        private HttpPostedFileBase _submittedfile { get; set; }

        public SubmissionEvaluator(StudentSubmissionViewModel model, HttpPostedFileBase file)
        {
            _submission = model;
            _submittedfile = file;
        }

        public bool Evaluate()
        {
            Random _randomNum = new Random();
            if (_randomNum.Next(100) < 50)
            {
                return true;  
            }
            
            else
            {
                return false;
            }
        }
    }
}