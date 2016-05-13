using Mooshack_2.Services;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshack_2.Models;
using Mooshack_2.Models.ViewModels;
using System.Net;
using Microsoft.AspNet.Identity;
using System.IO;
using Mooshack_2.Helpers;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;

namespace Mooshack_2.Controllers
{
    public class AssignmentController : Controller
    {
        AssignmentService _assignmentService;
        CourseService _courseService;

        public AssignmentController()
        {
            _assignmentService = new AssignmentService( null );
            _courseService = new CourseService( null );
        }

        // GET: Assignment
        public ActionResult index()
        {
            return View();
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult teacherAssignmentPage( int? courseID )
        {
            if( courseID != null )
            {
                var _courseName = _courseService.getCourseViewModelByID( courseID );
                var _assignmentModels = new TeacherAssignmentViewModel
                {
                    CourseName = _courseName.Name,
                    CourseID = courseID.Value,
                    Assignments = _assignmentService.getAssignmentByCourseID( courseID )
                };

                return View( _assignmentModels );
            }
            else
            {
                return View( "Error404" );
            }
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult teacherAssignmentMilestonesPage( int? assignmentID )
        {
            if (assignmentID != null)
            {
                AssignmentViewModel _assignment = _assignmentService.getAssignmentViewModelById(assignmentID.Value);
                var _course = _courseService.getCourseViewModelByAssignmentID(assignmentID.Value);
                _assignment.CourseName = _course.Name;
                return View(_assignment);
            }
            else
            {
                return View("Error404");
            }
        }

        [Authorize( Roles = "Student" )]
        public ActionResult studentAssignmentMilestonePage( int? assignmentID )
        {
            if(assignmentID != null)
            {
                AssignmentViewModel _assignment = _assignmentService.getAssignmentViewModelById(assignmentID.Value);
                var _course = _courseService.getCourseViewModelByAssignmentID(assignmentID.Value);
                _assignment.CourseName = _course.Name;
                return View(_assignment);
            }
            else
            {
                return View("Error404");
            }
            
        }

        public bool dateTimeValidator(DateTime startdate, DateTime enddate)
        {
            DateTime _startDateTime = new DateTime();
            DateTime _endDateTime = new DateTime();

            if (startdate > enddate)
            {
                return false;
            }

            else if (startdate.Year < 2000 || enddate.Year < 2000)
            {
                return false;
            }

            else if (startdate.Year > 9000 || enddate.Year > 9000)
            {
                return false;
            }

            else if (DateTime.TryParse(startdate.ToString(), out _startDateTime) == false)
            {
                return false;
            }
            else if (DateTime.TryParse(enddate.ToString(), out _endDateTime) == false)
            {
                return false;
            }

            return true;

        }

        public ActionResult CreateAssignment(int? courseID)
        {
            if(courseID != null)
            {
                CreateAssignmentViewModel _newAssignmentViewModel = new CreateAssignmentViewModel();
                _newAssignmentViewModel.CourseID = courseID.Value;

                return View(_newAssignmentViewModel);
            }

            return View("Error404");

        }


        [Authorize( Roles = "Teacher" )]
        [HttpPost]
        public ActionResult CreateAssignment( CreateAssignmentViewModel model )
        {
            if (ModelState.IsValid)
            {
                if (dateTimeValidator(model.StartDateTime, model.EndDateTime) == true)
                {
                    _assignmentService.createAssignment(model);
                    return RedirectToAction("TeacherAssignmentPage", "Assignment", new { courseID = model.CourseID });
                }

            }
            return View(model);
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult EditAssignment( int? assignmentID )
        {
            if(assignmentID != null)
            {
                var _assignment = _assignmentService.getAssignmentViewModelById(assignmentID.Value);
                var _editAssignmentViewModel = new EditAssignmentViewModel
                {
                    id = _assignment.id,
                    Title = _assignment.Title,
                    CourseID = _assignment.CourseID,
                    Description = _assignment.Description,
                    EndDateTime = _assignment.EndDateTime,
                    StartDateTime = _assignment.StartDateTime
                };

                return View(_editAssignmentViewModel);
            }
            else
            {
                return View("Error404");
            }
            

        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]

        public ActionResult EditAssignment(EditAssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (dateTimeValidator(model.StartDateTime, model.EndDateTime) == true)
                {
                    _assignmentService.editAssignment(model);
                    return RedirectToAction("TeacherAssignmentPage", "Assignment", new { courseID = model.CourseID });
                }
            }

            return View(model);
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult EditMilestone( int? milestoneID )
        {
            if( milestoneID == null )
            {
                return View("Error404");
            }

            var _milestone = _assignmentService.getEditMilestoneViewModelByID( milestoneID.Value );

            return View( _milestone );
        }

        [Authorize( Roles = "Teacher" )]
        [HttpPost]
        public ActionResult EditMilestone( EditMilestoneViewModel model )
        {
            if (ModelState.IsValid)
            {
                _assignmentService.editMilestone(model);
                return RedirectToAction("TeacherAssignmentMilestonesPage", "Assignment", new { assignmentID = model.AssignmentID });
            }

            return View(model);

        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult deleteAssignment( int assignmentID, int courseReturnID )
        {
            _assignmentService.deleteAssignment( assignmentID );

            return RedirectToAction( "teacherAssignmentPage", "Assignment", new {courseID = courseReturnID} );
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult deleteMilestone( int milestoneID, int assignmentReturnID )
        {
            AssignmentService _deleteMileStone = new AssignmentService( null );
            _deleteMileStone.deleteMilestone( milestoneID );

            return RedirectToAction( "teacherAssignmentMilestonesPage", "Assignment",
                new {assignmentID = assignmentReturnID} );
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult CreateMilestone( int? assignmentID )
        {
            if(assignmentID != null)
            {
                CreateMilestoneViewModel _newMileStone = new CreateMilestoneViewModel();
                _newMileStone.AssignmentID = assignmentID.Value;
                var _assignment = _assignmentService.getAssignmentViewModelById(assignmentID.Value);
                _newMileStone.AssignmentName = _assignment.Title;
                return View(_newMileStone);
            }

            return View("Error404");
        }

        [Authorize( Roles = "Teacher" )]
        [HttpPost]
        public ActionResult CreateMilestone( CreateMilestoneViewModel model )
        {
            _assignmentService.createAssignmentMilestone( model );

            return RedirectToAction( "teacherAssignmentMilestonesPage", "Assignment",
                new {assignmentID = model.AssignmentID} );
        }

        [Authorize( Roles = "Student" )]
        /*Student gets information about a single course*/
        public ActionResult studentAssignmentPage( int? courseID )
        {
            if( courseID != null )
            {
                var _assignmentService = new AssignmentService( null );
                var _courseName = _courseService.getCourseViewModelByID( courseID );

                var _assignmentModels = new StudentAssignmentViewModel
                {
                    CourseName = _courseName.Name,
                    CourseID = ( courseID.Value ),
                    Assignments = _assignmentService.getAssignmentByCourseID( courseID )
                };

                return View( _assignmentModels );
            }

            return View("Error404");
        }

        [Authorize( Roles = "Student" )]
        public ActionResult studentSubmitMilestone( int? milestoneID )
        {
            if( milestoneID == null )
            {
                return new HttpStatusCodeResult( HttpStatusCode.BadRequest );
            }

            var _studentSubmissonForm = new StudentSubmissionViewModel();
            var _milestone = _assignmentService.getMilestoneViewModelByID( milestoneID.Value );
            _studentSubmissonForm.StudentID = User.Identity.GetUserId();
            _studentSubmissonForm.MilestoneID = milestoneID.Value;
            _studentSubmissonForm.MilestoneTitle = _milestone.Title;
            _studentSubmissonForm.MilestoneDescription = _milestone.Description;

            return View( _studentSubmissonForm );
        }

        [Authorize( Roles = "Student" )]
        [HttpPost]
        public ActionResult studentSubmitMilestone( StudentSubmissionViewModel model, HttpPostedFileBase file )
        {
            string _currentpath = HttpContext.Server.MapPath( "~" );
            if( file.ContentLength >= 0 )
            {
                string _dir = _currentpath + "Submissions\\" + model.MilestoneID.ToString() + "\\" +
                              User.Identity.GetUserName();
                Directory.CreateDirectory( _dir );
                var _fileName = DateTime.Now.ToString( "yyyy-dd-M--HH-mm-ss" ) + Path.GetFileName( file.FileName );
                var _path = Path.Combine( _dir, _fileName );
                file.SaveAs( _path );
                model.FilePath = _path;
                model.DateTimeSubmitted = DateTime.Now;
                var _milestone = _assignmentService.getMilestoneByID( model.MilestoneID );
                SubmissionEvaluator _evaluator = new SubmissionEvaluator( file, _milestone.Input, _milestone.Output );
                model.Accepted = _evaluator.Evaluate();
                _assignmentService.addSubmission( model );
            }
            else
            {
                return View( "Error" );
            }

            return RedirectToAction( "viewStudentSubmissions", "Assignment", new {milestoneID = model.MilestoneID} );
        }

        [Authorize( Roles = "Student" )]
        public ActionResult viewStudentSubmissions( int? milestoneID )
        {
            if(milestoneID != null)
            {
                var _studentID = User.Identity.GetUserId();
                ViewSubmissions _viewSubmissions = _assignmentService.getAllSubmissionsByStudentID(_studentID,
                    milestoneID.Value);

                return View(_viewSubmissions);
            }

            return View("Error404");
            
        }

        [Authorize( Roles = "Teacher" )]
        public ActionResult viewAllStudentSubmissions( int? milestoneID, string sortOrder )
        {
            if(milestoneID == null)
            {
                return View("Error404");
            }

            ViewSubmissions _viewSubmissions = _assignmentService.getAllSubmissionsByMilestoneID( milestoneID.Value );
            var _userManager =
                new UserManager<ApplicationUser>( new UserStore<ApplicationUser>( new ApplicationDbContext() ) );

            foreach( var _submission in _viewSubmissions.Submissions )
            {
                var _student = _userManager.FindById( _submission.StudentID );
                _submission.StudentName = _student.UserName;
            }

            _viewSubmissions.MilestoneID = milestoneID.Value;

            switch( sortOrder )
            {
                case "id":
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderBy( x => x.id ).ToList();
                    break;
                case "name":
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderBy( x => x.StudentName ).ToList();
                    break;
                case "date":
                    _viewSubmissions.Submissions =
                        _viewSubmissions.Submissions.OrderBy( x => x.DateTimeSubmitted ).ToList();
                    break;
                case "accepted":
                    _viewSubmissions.Submissions =
                        _viewSubmissions.Submissions.OrderByDescending( x => x.Accepted ).ToList();
                    break;
                default:
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderBy( x => x.id ).ToList();
                    break;
            }

            return View( _viewSubmissions );
        }

        public ActionResult openSubmission( string currentPath )
        {
            byte[] _filedata = System.IO.File.ReadAllBytes( currentPath );
            string _contentType = MimeMapping.GetMimeMapping( currentPath );

            return ( File( _filedata, _contentType ) );
        }

        public ActionResult downloadSubmission( string currentPath )
        {
            byte[] _filedata = System.IO.File.ReadAllBytes( currentPath );
            string _contentType = MimeMapping.GetMimeMapping( currentPath );
            int _pos = currentPath.LastIndexOf( "\\" ) + 1;

            return File( _filedata, _contentType, currentPath.Substring( _pos, currentPath.Length - _pos ) );
        }
    }
}