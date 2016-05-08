using Mooshack_2.Models;
using Mooshack_2.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.Entities;

namespace Mooshack_2.Services
{
    public class AssignmentService
    {
        private ApplicationDbContext _dbContext;

        public AssignmentService()
        {
            _dbContext = new ApplicationDbContext();
        }


        public List<AssignmentViewModel> getAssignmentByCourseID(int? cid)
        {
            if (cid == null)
            {
                return null;
            }

            else
            {
                IEnumerable<Assignment> _assignments = (from item in _dbContext.Assignments
                                                        where item.CourseID == cid
                                                        select item).ToList();

                var _assignmentViewModels = new List<AssignmentViewModel>();
                foreach (var _assignment in _assignments)
                {
                    _assignmentViewModels.Add(new AssignmentViewModel
                    {
                        id = _assignment.id,
                        CourseID = _assignment.CourseID,
                        Description = _assignment.Description,
                        EndDateTime = _assignment.EndDateTime,
                        StartDateTime = _assignment.StartDateTime,
                        Title = _assignment.Title
                    });
                }

                return _assignmentViewModels;

            }

        }



        public List<Assignment> getAllAssignments()
        {
            var _allAssignments = _dbContext.Assignments.ToList();
            return _allAssignments;
        }

        public List<AssignmentViewModel> getAllAssignmentViewModels()
        {
            var _assignmentViewModels = new List<AssignmentViewModel>();
            var _assignments = getAllAssignments();
            foreach (var _assignment in _assignments)
            {
                _assignmentViewModels.Add(new AssignmentViewModel
                {
                    id = _assignment.id,
                    CourseID = _assignment.CourseID,
                    Description = _assignment.Description,
                    EndDateTime = _assignment.EndDateTime,
                    StartDateTime = _assignment.StartDateTime,
                    Title = _assignment.Title
                });
            }

            return _assignmentViewModels;
        }


    }
}