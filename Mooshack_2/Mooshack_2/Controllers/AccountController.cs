using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Mooshack_2.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Net.Mail;
using Mooshack_2.Services;
using Mooshack_2.Models.ViewModels;

namespace Mooshack_2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private List<SelectListItem> _userRoles;
        private ApplicationDbContext _dbContext;
        private AssignmentService _assignmentService;
        private CourseService _courseService;

        public AccountController()
        {
            _dbContext = new ApplicationDbContext();
            _userRoles = new List<SelectListItem>();
            _assignmentService = new AssignmentService( null );
            _courseService = new CourseService( null );
            _userRoles.Add( new SelectListItem {Text = "Administrator", Value = "Administrator"} );
            _userRoles.Add( new SelectListItem {Text = "Teacher", Value = "Teacher"} );
            _userRoles.Add( new SelectListItem {Text = "Student", Value = "Student", Selected = true} );
        }

        public AccountController( ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public ApplicationSignInManager signInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager userManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login( string returnUrl )
        {
            init();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login( LoginViewModel model, string returnUrl )
        {
            if( !ModelState.IsValid )
            {
                return View( model );
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var _email = new MailAddress( model.Email );
            var _userName = _email.User;
            var _result =
                await
                    signInManager.PasswordSignInAsync( _userName, model.Password, model.RememberMe, false );

            switch ( _result )
            {
                case SignInStatus.Success:
                    return redirectToLocal( returnUrl );
                case SignInStatus.LockedOut:
                    return View( "Lockout" );
                case SignInStatus.RequiresVerification:
                    return RedirectToAction( "SendCode", new {ReturnUrl = returnUrl, RememberMe = model.RememberMe} );
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError( "", "Invalid login attempt." );

                    return View( model );
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode( string provider, string returnUrl, bool rememberMe )
        {
            // Require that the user has already logged in via username/password or external login
            if( !await signInManager.HasBeenVerifiedAsync() )
            {
                return View( "Error" );
            }

            return View( new VerifyCodeViewModel {Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe} );
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode( VerifyCodeViewModel model )
        {
            if( !ModelState.IsValid )
            {
                return View( model );
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var _result =
                await
                    signInManager.TwoFactorSignInAsync( model.Provider, model.Code, model.RememberMe,
                        model.RememberBrowser );

            switch( _result )
            {
                case SignInStatus.Success:
                    return redirectToLocal( model.ReturnUrl );
                case SignInStatus.LockedOut:
                    return View( "Lockout" );
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError( "", "Invalid code." );
                    return View( model );
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register( RegisterViewModel model )
        {
            if( ModelState.IsValid )
            {
                var _user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                var _result = await userManager.CreateAsync( _user, model.Password );

                if( _result.Succeeded )
                {
                    await signInManager.SignInAsync( _user, false, false );

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction( "index", "Home" );
                }

                addErrors( _result );
            }

            // If we got this far, something failed, redisplay form
            return View( model );
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail( string userId, string code )
        {
            if( userId == null || code == null )
            {
                return View( "Error" );
            }

            var _result = await userManager.ConfirmEmailAsync( userId, code );

            return View( _result.Succeeded ? "ConfirmEmail" : "Error" );
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword( ForgotPasswordViewModel model )
        {
            if( ModelState.IsValid )
            {
                var _user = await userManager.FindByNameAsync( model.Email );

                if( _user == null || !await userManager.IsEmailConfirmedAsync( _user.Id ) )
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View( "ForgotPasswordConfirmation" );
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View( model );
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword( string code )
        {
            return code == null ? View( "Error" ) : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword( ResetPasswordViewModel model )
        {
            if( !ModelState.IsValid )
            {
                return View( model );
            }

            var _user = await userManager.FindByNameAsync( model.Email );

            if( _user == null )
            {
                // Don't reveal that the user does not exist
                return RedirectToAction( "ResetPasswordConfirmation", "Account" );
            }

            var _result = await userManager.ResetPasswordAsync( _user.Id, model.Code, model.Password );

            if( _result.Succeeded )
            {
                return RedirectToAction( "ResetPasswordConfirmation", "Account" );
            }

            addErrors( _result );

            return View();
        }

        [Authorize( Roles = "Administrator" )]
        public ActionResult adminResetPassword( string userID, string username )
        {
            var _model = new AdminResetPasswordViewModel();
            _model.UserID = userID;
            _model.UserName = username;

            return View( _model );
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> adminResetPassword( AdminResetPasswordViewModel model )
        {
            var _userManager =
                new UserManager<ApplicationUser>( new UserStore<ApplicationUser>( new ApplicationDbContext() ) );

            if( !ModelState.IsValid )
            {
                return View( model );
            }

            var _user = await _userManager.FindByIdAsync( model.UserID );

            if( _user == null )
            {
                return RedirectToAction( "showAllUsers", "Account" );
            }

            var _result = await _userManager.RemovePasswordAsync( _user.Id );

            if( _result.Succeeded )
            {
                _result = await _userManager.AddPasswordAsync( _user.Id, model.Password );

                if( _result.Succeeded )
                {
                    return RedirectToAction( "showAllUsers", "Account" );
                }
            }

            return View( model );
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin( string provider, string returnUrl )
        {
            // Request a redirect to the external login provider
            return new ChallengeResult( provider,
                Url.Action( "ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl} ) );
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode( string returnUrl, bool rememberMe )
        {
            var _userId = await signInManager.GetVerifiedUserIdAsync();

            if( _userId == null )
            {
                return View( "Error" );
            }

            var _userFactors = await userManager.GetValidTwoFactorProvidersAsync( _userId );
            var _factorOptions =
                _userFactors.Select( purpose => new SelectListItem {Text = purpose, Value = purpose} ).ToList();

            return View( new SendCodeViewModel {Providers = _factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe} );
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode( SendCodeViewModel model )
        {
            if( !ModelState.IsValid )
            {
                return View();
            }

            // Generate the token and send it
            if( !await signInManager.SendTwoFactorCodeAsync( model.SelectedProvider ) )
            {
                return View( "Error" );
            }

            return RedirectToAction( "VerifyCode",
                new {Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe} );
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback( string returnUrl )
        {
            var _loginInfo = await authenticationManager.GetExternalLoginInfoAsync();

            if( _loginInfo == null )
            {
                return RedirectToAction( "Login" );
            }

            // Sign in the user with this external login provider if the user already has a login
            var _result = await signInManager.ExternalSignInAsync( _loginInfo, false );

            switch( _result )
            {
                case SignInStatus.Success:
                    return redirectToLocal( returnUrl );
                case SignInStatus.LockedOut:
                    return View( "Lockout" );
                case SignInStatus.RequiresVerification:
                    return RedirectToAction( "SendCode", new {ReturnUrl = returnUrl, RememberMe = false} );
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = _loginInfo.Login.LoginProvider;

                    return View( "ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel {Email = _loginInfo.Email} );
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation( ExternalLoginConfirmationViewModel model,
            string returnUrl )
        {
            if( User.Identity.IsAuthenticated )
            {
                return RedirectToAction( "Index", "Manage" );
            }

            if( ModelState.IsValid )
            {
                // Get the information about the user from the external login provider
                var _info = await authenticationManager.GetExternalLoginInfoAsync();

                if( _info == null )
                {
                    return View( "ExternalLoginFailure" );
                }

                var _user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                var _result = await userManager.CreateAsync( _user );

                if( _result.Succeeded )
                {
                    _result = await userManager.AddLoginAsync( _user.Id, _info.Login );

                    if( _result.Succeeded )
                    {
                        await signInManager.SignInAsync( _user, false, false );

                        return redirectToLocal( returnUrl );
                    }
                }
                addErrors( _result );
            }
            ViewBag.ReturnUrl = returnUrl;

            return View( model );
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            authenticationManager.SignOut( DefaultAuthenticationTypes.ApplicationCookie );

            return RedirectToAction( "index", "Home" );
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( _userManager != null )
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if( _signInManager != null )
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose( disposing );
        }

        [Authorize( Roles = "Administrator" )]
        public ActionResult showAllUsers()
        {
            var _allUsers = _dbContext.Users.OrderBy( x => x.UserName ).ToList();
            var _allUsersViewModels = new List<UserViewModel>();
            var _userManager =
                new UserManager<ApplicationUser>( new UserStore<ApplicationUser>( new ApplicationDbContext() ) );

            foreach( var _user in _allUsers )
            {
                var _userRole = getUserRole( _userManager, _user.Id );
                _allUsersViewModels.Add( new UserViewModel
                {
                    Id = _user.Id,
                    UserName = _user.UserName,
                    Email = _user.Email,
                    Role = _userRole
                } );
            }

            ViewBag._currentUser = User.Identity.GetUserId();

            return View( _allUsersViewModels );
        }

        public string getUserRole( UserManager<ApplicationUser> usermanager, string userID )
        {
            if( usermanager.IsInRole( userID, "Student" ) )
            {
                return "Student";
            }
            else if( usermanager.IsInRole( userID, "Teacher" ) )
            {
                return "Teacher";
            }

            return "Administrator";
        }

        [Authorize( Roles = "Administrator" )]
        public ActionResult createUser()
        {
            ViewBag.UserRoles = _userRoles;

            return View();
        }

        [HttpPost]
        [Authorize( Roles = "Administrator" )]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> createUser( CreateUserViewModel model, string userRoles )
        {
            if( ModelState.IsValid )
            {
                var _email = new MailAddress( model.Email );
                var _userName = _email.User.ToLower();
                var _user = new ApplicationUser {UserName = _userName, Email = model.Email.ToLower()};
                var _result = await userManager.CreateAsync( _user, model.Password );

                if( _result.Succeeded )
                {
                    userManager.AddToRole( _user.Id, userRoles );
                    return RedirectToAction( "index", "Home" );
                }

                addErrors( _result );
            }

            ViewBag.UserRoles = _userRoles;

            return View( model );
        }

        [Authorize( Roles = "Administrator" )]
        public async Task<ActionResult> removeUser( string userID, string role )
        {
            var _userManager =
                new UserManager<ApplicationUser>( new UserStore<ApplicationUser>( new ApplicationDbContext() ) );
            var _user = _userManager.FindByIdAsync( userID ).Result;
            _assignmentService.deleteSubmissionsByStudentID( userID );

            if( role == "Teacher" )
            {
                var _allCoursesWithTeacher = (from _teacher in _dbContext.CourseTeacher
                                              where _teacher.TeacherID == userID
                                              select _teacher).ToList();

                foreach ( var _teacher in _allCoursesWithTeacher )
                {
                    _courseService.removeTeacherFromCourse( _teacher.TeacherID, _teacher.CourseID );
                }
            }
            else if( role == "Student" )
            {
                var _allCoursesWithStudent = (from _student in _dbContext.CourseStudent
                                              where _student.StudentID == userID
                                              select _student).ToList();

                foreach ( var _student in _allCoursesWithStudent )
                {
                    _courseService.removeStudentFromCourse( _student.StudentID, _student.CourseID );
                }
            }

            if( _user != null )
            {
                await _userManager.DeleteAsync( _user );
            }

            return RedirectToAction( "showAllUsers" );
        }

        public void init()
        {
            var _roleManager = new RoleManager<IdentityRole>( new RoleStore<IdentityRole>( new ApplicationDbContext() ) );
            var _userManager =
                new UserManager<ApplicationUser>( new UserStore<ApplicationUser>( new ApplicationDbContext() ) );

            if( !_roleManager.RoleExists( "Administrator" ) )
            {
                _roleManager.Create( new IdentityRole( "Administrator" ) );
            }

            if( !_roleManager.RoleExists( "Teacher" ) )
            {
                _roleManager.Create( new IdentityRole( "Teacher" ) );
            }

            if( !_roleManager.RoleExists( "Student" ) )
            {
                _roleManager.Create( new IdentityRole( "Student" ) );
            }

            var _userID = _userManager.FindByEmail( "admin@admin.is" );

            if( _userID != null )
            {
                if( !_userManager.IsInRole( _userID.Id, "Administrator" ) )
                {
                    _userManager.AddToRole( _userID.Id, "Administrator" );
                }
            }
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager authenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void addErrors( IdentityResult result )
        {
            foreach( var _error in result.Errors )
            {
                ModelState.AddModelError( "", _error );
            }
        }

        private ActionResult redirectToLocal( string returnUrl )
        {
            if( Url.IsLocalUrl( returnUrl ) )
            {
                return Redirect( returnUrl );
            }

            return RedirectToAction( "index", "Home" );
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult( string provider, string redirectUri )
                : this( provider, redirectUri, null )
            {
            }

            public ChallengeResult( string provider, string redirectUri, string userId )
            {
                loginProvider = provider;
                this.redirectUri = redirectUri;
                this.userID = userId;
            }

            public string loginProvider { get; set; }
            public string redirectUri { get; set; }
            public string userID { get; set; }

            public override void ExecuteResult( ControllerContext context )
            {
                var _properties = new AuthenticationProperties {RedirectUri = redirectUri};

                if( userID != null )
                {
                    _properties.Dictionary[XsrfKey] = userID;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge( _properties, loginProvider );
            }
        }
        #endregion
    }
}