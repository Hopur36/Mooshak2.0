using Mooshack_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshack_2.Models;
using Mooshack_2.Models.ViewModels;
using System.Net;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Mooshack_2.Controllers
{
    public class AssignmentController : Controller
    {
        AssignmentService _assignmentService;
        CourseService _courseService;

        public AssignmentController()
        {
            _assignmentService = new AssignmentService();
            _courseService = new CourseService();
        }

        // GET: Assignment
        public ActionResult Index()
        {

            return View();
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult TeacherAssignmentPage(int? courseID)
        {
            if (courseID != null)
            {
                var _assignmentService = new AssignmentService();
                var _courseName = _courseService.getCourseViewModelByID(courseID);
                var _assignmentModels = new TeacherAssignmentViewModel
                {
                    CourseName = _courseName.Name,
                    CourseID = Convert.ToInt32(courseID),
                    Assignments = _assignmentService.getAssignmentByCourseID(courseID)
                };


                return View(_assignmentModels);

            }

            return View();
        }

     

        [Authorize(Roles = "Teacher")]
        public ActionResult TeacherNewAssignmentPage()
        {
            return View();
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult TeacherAssignmentMilestonesPage(int assignmentID)
        {
            AssignmentViewModel _assignment = _assignmentService.GetAssignmentViewModelByID(assignmentID);
            return View(_assignment);
        }

        [Authorize(Roles = "Student")]
        public ActionResult StudentAssignmentMilestonePage(int assignmentID)
        {
            AssignmentViewModel _assignment = _assignmentService.GetAssignmentViewModelByID(assignmentID);
            return View(_assignment);
        }

        public ActionResult CreateAssignment(int CourseID)
        {
            CreateAssignmentViewModel _newAssignmentViewModel = new CreateAssignmentViewModel();
            _newAssignmentViewModel.CourseID = CourseID;
            return View(_newAssignmentViewModel);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public ActionResult CreateAssignment(CreateAssignmentViewModel model)
        {
            _assignmentService.CreateAssignment(model);


            return RedirectToAction("TeacherAssignmentPage", "Assignment", new { courseID = model.CourseID });

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
            return RedirectToAction("TeacherAssignmentPage", "Assignment", new { courseID = model.CourseID });
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
            return RedirectToAction("TeacherAssignmentMilestonesPage", "Assignment", new { assignmentID = model.AssignmentID });
        }




        [Authorize(Roles = "Teacher")]
        public ActionResult DeleteAssignment(int assignmentID, int courseReturnID)
        {
            _assignmentService.DeleteAssignment(assignmentID);
            return RedirectToAction("TeacherAssignmentPage", "Assignment", new { courseID = courseReturnID });
        }

        [Authorize(Roles = "Teacher")]
        public ActionResult CreateMilestone(int assignmentID)
        {
            CreateMilestoneViewModel _newMileStone = new CreateMilestoneViewModel();
            _newMileStone.AssignmentID = assignmentID;

            return View(_newMileStone);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public ActionResult CreateMilestone(CreateMilestoneViewModel model)
        {
            _assignmentService.CreateAssignmentMilestone(model);

            return RedirectToAction("TeacherAssignmentMilestonesPage", "Assignment", new { assignmentID = model.AssignmentID });
        }


        [Authorize(Roles = "Student")]
        /*Student gets information about a single course*/
        public ActionResult studentAssignmentPage(int? courseID)
        {
            if (courseID != null)
            {
                var _assignmentService = new AssignmentService();
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
        public ActionResult studentSubmitMilestone(StudentSubmissionViewModel model,HttpPostedFileBase file)
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

                _assignmentService.addSubmission(model);
            }

            else
            {
                return View("Error");
            }

            return RedirectToAction("Index", "Home");



        }
    }
}