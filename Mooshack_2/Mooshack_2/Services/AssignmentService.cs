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
        /// <summary>
        /// Variable can not be changed, unless it is in a constructor
        /// </summary>
        private readonly IMyDataContext _dbContext;

        /// <summary>
        /// Constructor for AssignmentService,
        /// if argument is null then new ApplicationDbContext() is used
        /// </summary>
        /// <param name="context"></param>
        public AssignmentService(IMyDataContext context)
        {
            _dbContext = context ?? new ApplicationDbContext();
        }

        /* private ApplicationDbContext _dbContext;

         public AssignmentService()
         {
             _dbContext = new ApplicationDbContext();
         }*/

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

                String _coursename = (from item in _dbContext.Courses
                                      where item.ID == cid
                                      select item.Name).FirstOrDefault();

                var _assignmentViewModels = new List<AssignmentViewModel>();

                foreach (var _assignment in _assignments)
                {
                    _assignmentViewModels.Add(new AssignmentViewModel
                    {
                        id = _assignment.id,
                        CourseID = _assignment.CourseID,
                        CourseName = _coursename,
                        Description = _assignment.Description,
                        EndDateTime = _assignment.EndDateTime,
                        StartDateTime = _assignment.StartDateTime,
                        Title = _assignment.Title,
                        Milestones = getAllMilestonesByAssignmentID(_assignment.id)
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
                    Title = _assignment.Title,
                    Milestones = getAllMilestonesByAssignmentID(_assignment.id)
                    
                });
            }
            return _assignmentViewModels;
        }

        public AssignmentViewModel GetAssignmentViewModelByID(int aID)
        {
            var _assignment = (from item in _dbContext.Assignments
                               where item.id == aID
                               select item).FirstOrDefault();

            var _courseService = new CourseService(null);
            var _course = _courseService.getCourseViewModelByID(_assignment.CourseID);

            AssignmentViewModel _assignmentViewModel = new AssignmentViewModel
            {
                id = _assignment.id,
                CourseID = _assignment.CourseID,
                Description = _assignment.Description,
                EndDateTime = _assignment.EndDateTime,
                StartDateTime = _assignment.StartDateTime,
                Title = _assignment.Title,
                Milestones = getAllMilestonesByAssignmentID(_assignment.id)
            };
            return _assignmentViewModel;
        }

        public List<MilestoneViewModel> getAllMilestonesByAssignmentID(int aID)
        {
            var _allMilestones = (from item in _dbContext.Milestones
                                  where item.AssignmentID == aID
                                  select item).ToList();

            List<MilestoneViewModel> _allMilestonesViewModel = new List<MilestoneViewModel>();

            foreach (var item in _allMilestones)
            {
                _allMilestonesViewModel.Add(new MilestoneViewModel
                {
                    id = item.id,
                    Title = item.Title,
                    Description = item.Description,
                    Weight = item.Weight
                });
            }

            return _allMilestonesViewModel;
        }

        public bool CreateAssignment(CreateAssignmentViewModel model)
        {
            Assignment _newAssignment = new Assignment {CourseID = model.CourseID,Title = model.Title,
            Description = model.Description, StartDateTime = model.StartDateTime, EndDateTime = model.EndDateTime};
            _dbContext.Assignments.Add(_newAssignment);
            _dbContext.SaveChanges();

            return true;
        }

        public bool DeleteAssignment(int assignmentID)
        {
            List<Milestone> _milestones = new List<Milestone>();

            _milestones = (from milestone in _dbContext.Milestones
                           where milestone.AssignmentID == assignmentID
                           select milestone).ToList();

            foreach (var milestone in _milestones)
            {
                _dbContext.Milestones.Remove(milestone);
                _dbContext.SaveChanges();
            }

            Assignment _deletedAssignment = (from assignment in _dbContext.Assignments
                                             where assignment.id == assignmentID
                                             select assignment).FirstOrDefault();
            _dbContext.Assignments.Remove(_deletedAssignment);
            _dbContext.SaveChanges();

            return true;
        }

        public bool CreateAssignmentMilestone(CreateMilestoneViewModel model)
        {
            Milestone _newMilestone = new Milestone
            {
                AssignmentID = model.AssignmentID,
                Title = model.Title,
                Description = model.Description,
                Weight = model.Weight,
                Input = model.Input,
                Output = model.Output
            };

            _dbContext.Milestones.Add(_newMilestone);
            _dbContext.SaveChanges();

            return true;
        }

        public List<AssignmentViewModel> getActiveAssignmentByCourseID(int? cid)
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

                String _courseName = (from item in _dbContext.Courses
                                      where item.ID == cid
                                      select item.Name).FirstOrDefault();

                var _assignmentViewModels = new List<AssignmentViewModel>();

                foreach (var _assignment in _assignments)
                {
                        if (_assignment.EndDateTime > DateTime.Now)
                        {
                            _assignmentViewModels.Add(new AssignmentViewModel
                            {
                                id = _assignment.id,
                                CourseID = _assignment.CourseID,
                                CourseName = _courseName,
                                Description = _assignment.Description,
                                EndDateTime = _assignment.EndDateTime,
                                StartDateTime = _assignment.StartDateTime,
                                Title = _assignment.Title,
                                Milestones = getAllMilestonesByAssignmentID(_assignment.id)
                            });
                        }
                }

                return _assignmentViewModels;
            }



        }


        public bool EditAssignment(AssignmentViewModel assignment)
        {
            Assignment model = (from item in _dbContext.Assignments
                             where item.id == assignment.id
                             select item).SingleOrDefault();
    
            model.Title = assignment.Title;
            model.Description = assignment.Description;
            model.StartDateTime = assignment.StartDateTime;
            model.EndDateTime = assignment.EndDateTime;
            _dbContext.SaveChanges();

            return true;
        }

        public bool EditMilestone(EditMilestoneViewModel milestone)
        {
            Milestone model = (from item in _dbContext.Milestones
                                where item.id == milestone.id
                                select item).SingleOrDefault();

            model.Title = milestone.Title;
            model.Description = milestone.Description;
            model.Weight = milestone.Weight;
            model.Input = milestone.Input;
            model.Output = milestone.Output;

            _dbContext.SaveChanges();

            return true;
        }

        public EditMilestoneViewModel getEditMilestoneViewModelByID(int mID)
        {
           var _milestone = (from item in _dbContext.Milestones
                          where item.id == mID
                          select item).FirstOrDefault();

            var _milestoneViewModel = new EditMilestoneViewModel
            {
                id = _milestone.id,
                Title = _milestone.Title,
                AssignmentID = _milestone.AssignmentID,
                Description = _milestone.Description,
                Weight = _milestone.Weight,
                Input = _milestone.Input,
                Output = _milestone.Output
            };

            return _milestoneViewModel;

        }

        public MilestoneViewModel getMilestoneViewModelByID(int mID)
        {
            var _milestone = (from item in _dbContext.Milestones
                              where item.id == mID
                              select item).FirstOrDefault();

            var _milestoneViewModel = new MilestoneViewModel
            {
                id = _milestone.id,
                Title = _milestone.Title,
                Description = _milestone.Description,
                Weight = _milestone.Weight
            };

            return _milestoneViewModel;
        }

        public bool addSubmission(StudentSubmissionViewModel model)
        {
            Submission newSubmission = new Submission {
                id = model.id,
                MilestoneID = model.MilestoneID,
                StudentID = model.StudentID,
                DateTimeSubmitted = model.DateTimeSubmitted,
                ItemSubmittedPath = model.FilePath,
                Accepted = model.Accepted

            };

            _dbContext.Submissions.Add(newSubmission);
            _dbContext.SaveChanges();

            return true;
        }

        public ViewSubmissions getAllSubmissionsByStudentID(string studentID, int milestoneID)
        {
            ViewSubmissions _submissonsViewModelsByStudentID = new ViewSubmissions();
            List<Submission> _allSubmissonsByStudentID = new List<Submission>();
            List<StudentSubmissionViewModel> _allStudentSubmissionViewModel = new List<StudentSubmissionViewModel> ();
            if (studentID != null)
            {
                _allSubmissonsByStudentID = (from item in _dbContext.Submissions
                                             where item.StudentID == studentID
                                             where item.MilestoneID == milestoneID
                                             select item).ToList();
            }

            foreach (var submission in _allSubmissonsByStudentID)
            {
                _allStudentSubmissionViewModel.Add(new StudentSubmissionViewModel
                {
                    id = submission.id,
                    MilestoneID = submission.MilestoneID,
                    StudentID = submission.StudentID,
                    FilePath = submission.ItemSubmittedPath,
                    Accepted = submission.Accepted,
                    DateTimeSubmitted = submission.DateTimeSubmitted

                });
            }

            _submissonsViewModelsByStudentID.Submissions = _allStudentSubmissionViewModel;



            return _submissonsViewModelsByStudentID;
        }

        public ViewSubmissions getAllSubmissionsByMilestoneID(int milestoneID)
        {
            ViewSubmissions _submissonsViewModelsByMilestoneID = new ViewSubmissions();
            List<Submission> _allSubmissonsByMilestoneID = new List<Submission>();
            List<StudentSubmissionViewModel> _allStudentSubmissionViewModel = new List<StudentSubmissionViewModel>();

            _allSubmissonsByMilestoneID = (from item in _dbContext.Submissions
                                             where item.MilestoneID == milestoneID
                                             select item).ToList();
            

            foreach (var submission in _allSubmissonsByMilestoneID)
            {
                _allStudentSubmissionViewModel.Add(new StudentSubmissionViewModel
                {
                    id = submission.id,
                    MilestoneID = submission.MilestoneID,
                    StudentID = submission.StudentID,
                    FilePath = submission.ItemSubmittedPath,
                    Accepted = submission.Accepted,
                    DateTimeSubmitted = submission.DateTimeSubmitted

                });
            }

            _submissonsViewModelsByMilestoneID.Submissions = _allStudentSubmissionViewModel;



            return _submissonsViewModelsByMilestoneID;

        }
        public bool DeleteMilestone(int milestoneID)
        {
            Milestone _deletedMilestone = (from milestone in _dbContext.Milestones
                                           where milestone.id == milestoneID
                                           select milestone).FirstOrDefault();
            _dbContext.Milestones.Remove(_deletedMilestone);
            _dbContext.SaveChanges();

            return true;
        }


    }
}