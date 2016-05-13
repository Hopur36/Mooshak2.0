﻿using Mooshack_2.Services;
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

namespace Mooshack_2.Controllers
{
    public class AssignmentController : Controller
    {
        AssignmentService _assignmentService;
        CourseService _courseService;

        public AssignmentController()
        {
            _assignmentService = new AssignmentService(null);
            _courseService = new CourseService(null);
        }

        // GET: Assignment
        public ActionResult index()
        {
            return View();
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult teacherAssignmentPage(int? courseID)
        {
            if (courseID != null)
            {
                var _assignmentService = new AssignmentService(null);
                var _courseName = _courseService.getCourseViewModelByID(courseID);
                var _assignmentModels = new TeacherAssignmentViewModel
                {
                    CourseName = _courseName.Name,
                    CourseID = courseID.Value,
                    Assignments = _assignmentService.getAssignmentByCourseID(courseID)
                };

                return View(_assignmentModels);
            }
            else
            {
                return View("Error404");
            }

        }

        [Authorize(Roles = "Teacher")]
        public ActionResult teacherAssignmentMilestonesPage(int assignmentID)
        {
            AssignmentViewModel _assignment = _assignmentService.GetAssignmentViewModelByID(assignmentID);
            var _course = _courseService.getCourseViewModelByAssignmentID(assignmentID);
            _assignment.CourseName = _course.Name;

            return View(_assignment);
        }

        [Authorize(Roles = "Student")]
        public ActionResult studentAssignmentMilestonePage(int assignmentID)
        {
            AssignmentViewModel _assignment = _assignmentService.GetAssignmentViewModelByID(assignmentID);
            var _course = _courseService.getCourseViewModelByAssignmentID(assignmentID);
            _assignment.CourseName = _course.Name;

            return View(_assignment);
        }

        public ActionResult CreateAssignment(int courseID)
        {
            CreateAssignmentViewModel _newAssignmentViewModel = new CreateAssignmentViewModel();
            _newAssignmentViewModel.CourseID = courseID;

            return View(_newAssignmentViewModel);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public ActionResult CreateAssignment(CreateAssignmentViewModel model)
        {
            _assignmentService.CreateAssignment(model);

            return RedirectToAction("teacherAssignmentPage", "Assignment", new { courseID = model.CourseID });
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult EditAssignment(int assignmentID)
        {
            var _assignment = _assignmentService.GetAssignmentViewModelByID(assignmentID);

            return View(_assignment);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public ActionResult EditAssignment(AssignmentViewModel model)
        {
            _assignmentService.EditAssignment(model);

            return RedirectToAction("teacherAssignmentPage", "Assignment", new { courseID = model.CourseID });
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult EditMilestone(int? milestoneID)
        {
            if (milestoneID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var _milestone = _assignmentService.getEditMilestoneViewModelByID(milestoneID.Value);

            return View(_milestone);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public ActionResult EditMilestone(EditMilestoneViewModel model)
        {
            _assignmentService.EditMilestone(model);

            return RedirectToAction("teacherAssignmentMilestonesPage", "Assignment", new { assignmentID = model.AssignmentID });
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult deleteAssignment(int assignmentID, int courseReturnID)
        {
            _assignmentService.DeleteAssignment(assignmentID);

            return RedirectToAction("teacherAssignmentPage", "Assignment", new { courseID = courseReturnID });
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult deleteMilestone(int milestoneID, int assignmentReturnID)
        {
            AssignmentService _deleteMileStone = new AssignmentService(null);
            _deleteMileStone.DeleteMilestone(milestoneID);

            return RedirectToAction("teacherAssignmentMilestonesPage", "Assignment", new { assignmentID = assignmentReturnID });
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult CreateMilestone(int assignmentID)
        {
            CreateMilestoneViewModel _newMileStone = new CreateMilestoneViewModel();
            _newMileStone.AssignmentID = assignmentID;
            var _assignment = _assignmentService.GetAssignmentViewModelByID(assignmentID);
            _newMileStone.AssignmentName = _assignment.Title;
            return View(_newMileStone);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public ActionResult CreateMilestone(CreateMilestoneViewModel model)
        {
            _assignmentService.CreateAssignmentMilestone(model);

            return RedirectToAction("teacherAssignmentMilestonesPage", "Assignment", new { assignmentID = model.AssignmentID });
        }

        [Authorize(Roles = "Student")]
        /*Student gets information about a single course*/
        public ActionResult studentAssignmentPage(int? courseID)
        {
            if (courseID != null)
            {
                var _assignmentService = new AssignmentService(null);
                var _courseName = _courseService.getCourseViewModelByID(courseID);

                var _assignmentModels = new StudentAssignmentViewModel
                {
                    CourseName = _courseName.Name,
                    CourseID = (courseID.Value),
                    Assignments = _assignmentService.getAssignmentByCourseID(courseID)
                };

                return View(_assignmentModels);
            }

            return View();
        }

        [Authorize(Roles = "Student")]
        public ActionResult studentSubmitMilestone(int? milestoneID)
        {
            if (milestoneID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var _studentSubmissonForm = new StudentSubmissionViewModel();
            var _milestone = _assignmentService.getMilestoneViewModelByID(milestoneID.Value);
            _studentSubmissonForm.StudentID = User.Identity.GetUserId();
            _studentSubmissonForm.MilestoneID = milestoneID.Value;
            _studentSubmissonForm.MilestoneTitle = _milestone.Title;
            _studentSubmissonForm.MilestoneDescription = _milestone.Description;

            return View(_studentSubmissonForm);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        public ActionResult studentSubmitMilestone(StudentSubmissionViewModel model, HttpPostedFileBase file)
        {
            string _currentpath = HttpContext.Server.MapPath("~");
            if (file.ContentLength >= 0)
            {
                string _dir = _currentpath + "Submissions\\" + model.MilestoneID.ToString() + "\\" + User.Identity.GetUserName();
                Directory.CreateDirectory(_dir);
                var _fileName = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + Path.GetFileName(file.FileName);
                var _path = Path.Combine(_dir, _fileName);
                file.SaveAs(_path);
                model.FilePath = _path;
                model.DateTimeSubmitted = DateTime.Now;
                var _milestone = _assignmentService.getMilestoneByID(model.MilestoneID);
                SubmissionEvaluator _evaluator = new SubmissionEvaluator(file, _milestone.Input, _milestone.Output);
                model.Accepted = _evaluator.Evaluate();
                _assignmentService.addSubmission(model);
            }
            else
            {
                return View("Error");
            }

            return RedirectToAction("viewStudentSubmissions", "Assignment", new { milestoneID = model.MilestoneID });
        }

        [Authorize(Roles = "Student")]
        public ActionResult viewStudentSubmissions(int? milestoneID)
        {
            var _studentID = User.Identity.GetUserId();
            ViewSubmissions _viewSubmissions = _assignmentService.getAllSubmissionsByStudentID(_studentID, milestoneID.Value);

            return View(_viewSubmissions);
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult viewAllStudentSubmissions(int? milestoneID, string sortOrder)
        {
            ViewSubmissions _viewSubmissions = _assignmentService.getAllSubmissionsByMilestoneID(milestoneID.Value);
            var _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            foreach (var _submission in _viewSubmissions.Submissions)
            {
                var _student = _userManager.FindById(_submission.StudentID);
                _submission.StudentName = _student.UserName;
            }

            _viewSubmissions.MilestoneID = milestoneID.Value;

            switch (sortOrder)
            {
                case "id":
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderBy(x => x.id).ToList();
                    break;
                case "name":
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderBy(x => x.StudentName).ToList();
                    break;
                case "date":
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderBy(x => x.DateTimeSubmitted).ToList();
                    break;
                case "accepted":
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderByDescending(x => x.Accepted).ToList();
                    break;
                default:
                    _viewSubmissions.Submissions = _viewSubmissions.Submissions.OrderBy(x => x.id).ToList();
                    break;
            }

            return View(_viewSubmissions);
        }

        public ActionResult openSubmission(string currentPath)
        {
            byte[] _filedata = System.IO.File.ReadAllBytes(currentPath);
            string _contentType = MimeMapping.GetMimeMapping(currentPath);

            return (File(_filedata, _contentType));
        }

        public ActionResult downloadSubmission(string currentPath)
        {
            byte[] _filedata = System.IO.File.ReadAllBytes(currentPath);
            string _contentType = MimeMapping.GetMimeMapping(currentPath);
            int _pos = currentPath.LastIndexOf("\\") + 1;

            return File(_filedata, _contentType, currentPath.Substring(_pos, currentPath.Length - _pos));
        }
    }
}