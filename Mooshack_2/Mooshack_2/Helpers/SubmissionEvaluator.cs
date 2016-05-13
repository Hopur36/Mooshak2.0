using Mooshack_2.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshack_2.Helpers
{
    public class SubmissionEvaluator
    {
        private HttpPostedFileBase _submittedfile { get; set; }
        private string _input { get; set; }
        private string _expectedOutput { get; set; }

        public SubmissionEvaluator(HttpPostedFileBase file,string input,string output)
        {
            _submittedfile = file;
            _input = input;
            _expectedOutput = output;
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