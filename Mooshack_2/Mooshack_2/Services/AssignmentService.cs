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


        /// <summary>
        /// Gets all assignments by course ID
        /// </summary>
        /// <param name="cid"></param>
        /// <returns>List of assignment Viewmodels</returns>
        public List<AssignmentViewModel> getAssignmentByCourseID(int? cid)
        {
            if (cid == null)
            {
                return null;
            }
            else
            {
                //gets all assignments that have the matching course id
                IEnumerable<Assignment> _assignments = (from item in _dbContext.Assignments
                                                        where item.CourseID == cid
                                                        select item).ToList();

                //gets the name of the course
                String _coursename = (from item in _dbContext.Courses
                                      where item.ID == cid
                                      select item.Name).FirstOrDefault();

                var _assignmentViewModels = new List<AssignmentViewModel>();

                //loops through the list of assignments and adds a viewmodel for each assignment to the list
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


        /// <summary>
        /// Gets all assignments
        /// </summary>
        /// <returns>List of all assignments</returns>
        public List<Assignment> getAllAssignments()
        {
            var _allAssignments = _dbContext.Assignments.ToList();
            return _allAssignments;
        }

        /// <summary>
        /// Gets all assignments and adds to a Viewmodel and adds that Viewmodel to a list
        /// </summary>
        /// <returns>A List of assignmentViewmodels</returns>
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


        /// <summary>
        /// Gets an assignment that matches the given id
        /// </summary>
        /// <param name="aID"></param>
        /// <returns>Assignment Viewmodel</returns>
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

        /// <summary>
        /// Gets all milestones that match the given id and adds it to a list of milestone Viewmodels
        /// </summary>
        /// <param name="aID"></param>
        /// <returns>A list of Milestone ViewModels</returns>
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

        /// <summary>
        /// Gets a Viewmodel containing information given by the user and creates an assignment with that information
        /// </summary>
        /// <param name="model"></param>
        public void CreateAssignment(CreateAssignmentViewModel model)
        {
            Assignment _newAssignment = new Assignment{
                CourseID = model.CourseID,
                Title = model.Title,
                Description = model.Description,
                StartDateTime = model.StartDateTime,
                EndDateTime = model.EndDateTime
            };

            _dbContext.Assignments.Add(_newAssignment);
            _dbContext.SaveChanges();
        }

        //finnur alla milestones með gefnu assignment id og eyðir þeim og finnur að lokum assignment með gefnu id og eyðir því.
        /// <summary>
        /// Finds all milestones with the given assignment id, deletes them and then deletes the assignment itself
        /// </summary>
        /// <param name="assignmentID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a milestone for an assignment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all active assignments in a course with the given id
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
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

                //adds all assignments with an enddatetime that hasn't passed, to a list of assignment viewmodels
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

        /// <summary>
        /// Gets an assignment model from the controller, fetches the current version of the assignment from the database and updates it with 
        /// the new information from the assignment model
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a milestone model from the controller, fetches the current version of the milestone from the database and updates it with 
        /// the new information from the milestone model
        /// </summary>
        /// <param name="milestone"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a viewmodel of a milestone that is about to get edited
        /// </summary>
        /// <param name="mID"></param>
        /// <returns>EditMilestoneViewmodel</returns>
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

        /// <summary>
        /// gets a milestoneviewmodel by a specific id
        /// </summary>
        /// <param name="mID"></param>
        /// <returns>MilestoneViewModel</returns>
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
        /// <summary>
        /// gets a milestone by a specific id
        /// </summary>
        /// <param name="mID"></param>
        /// <returns></returns>
        public Milestone getMilestoneByID(int mID)
        {
            var _milestone = (from item in _dbContext.Milestones
                              where item.id == mID
                              select item).FirstOrDefault();
            return _milestone;
        }

        /// <summary>
        /// gets a model containing information about the submission and adds a new submission to the database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Gets all submissions that the given student has submitted for a specific milestone,
        /// adds it to a viewmodel and returns it
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="milestoneID"></param>
        /// <returns>List of StudentSubmissionsViewModel</returns>
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

        /// <summary>
        /// Gets all submissions for a specific milestone
        /// </summary>
        /// <param name="milestoneID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the milestone that matches the given id
        /// </summary>
        /// <param name="milestoneID"></param>
        /// <returns></returns>
        public bool DeleteMilestone(int milestoneID)
        {
            Milestone _deletedMilestone = (from milestone in _dbContext.Milestones
                                           where milestone.id == milestoneID
                                           select milestone).FirstOrDefault();
            _dbContext.Milestones.Remove(_deletedMilestone);
            _dbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Finds all submissions by a student and deletes them
        /// </summary>
        /// <param name="studentID"></param>
        public void deleteSubmissionsByStudentID(string studentID)
        {
            List<Submission> _allSubmissions = (from submissions in _dbContext.Submissions
                                                where submissions.StudentID == studentID
                                                select submissions).ToList();
            foreach (var submission in _allSubmissions)
            {
                _dbContext.Submissions.Remove(submission);
                _dbContext.SaveChanges();
            }
        }


    }
}