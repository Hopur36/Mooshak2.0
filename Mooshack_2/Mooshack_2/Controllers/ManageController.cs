using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Mooshack_2.Models;

namespace Mooshack_2.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController( ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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
        // GET: /Manage/index
        public async Task<ActionResult> index( ManageMessageId? message )
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess
                    ? "Your password has been changed."
                    : message == ManageMessageId.SetPasswordSuccess
                        ? "Your password has been set."
                        : message == ManageMessageId.SetTwoFactorSuccess
                            ? "Your two-factor authentication provider has been set."
                            : message == ManageMessageId.Error
                                ? "An error has occurred."
                                : message == ManageMessageId.AddPhoneSuccess
                                    ? "Your phone number was added."
                                    : message == ManageMessageId.RemovePhoneSuccess
                                        ? "Your phone number was removed."
                                        : "";

            var _userID = User.Identity.GetUserId();
            var _model = new IndexViewModel
            {
                HasPassword = hasPassword(),
                PhoneNumber = await userManager.GetPhoneNumberAsync( _userID ),
                TwoFactor = await userManager.GetTwoFactorEnabledAsync( _userID ),
                Logins = await userManager.GetLoginsAsync( _userID ),
                BrowserRemembered = await authenticationManager.TwoFactorBrowserRememberedAsync( _userID )
            };

            return View( _model );
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> removeLogin( string loginProvider, string providerKey )
        {
            ManageMessageId? _message;
            var _result =
                await
                    userManager.RemoveLoginAsync( User.Identity.GetUserId(),
                        new UserLoginInfo( loginProvider, providerKey ) );

            if( _result.Succeeded )
            {
                var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );
                if( _user != null )
                {
                    await signInManager.SignInAsync( _user, isPersistent: false, rememberBrowser: false );
                }
                _message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                _message = ManageMessageId.Error;
            }

            return RedirectToAction( "manageLogins", new {Message = _message} );
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber( AddPhoneNumberViewModel model )
        {
            if( !ModelState.IsValid )
            {
                return View( model );
            }

            // Generate the token and send it
            var _code = await userManager.GenerateChangePhoneNumberTokenAsync( User.Identity.GetUserId(), model.Number );

            if ( userManager.SmsService != null )
            {
                var _message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + _code
                };
                await userManager.SmsService.SendAsync( _message );
            }

            return RedirectToAction( "VerifyPhoneNumber", new {PhoneNumber = model.Number} );
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> enableTwoFactorAuthentication()
        {
            await userManager.SetTwoFactorEnabledAsync( User.Identity.GetUserId(), true );
            var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );

            if ( _user != null )
            {
                await signInManager.SignInAsync( _user, isPersistent: false, rememberBrowser: false );
            }

            return RedirectToAction( "index", "Manage" );
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> disableTwoFactorAuthentication()
        {
            await userManager.SetTwoFactorEnabledAsync( User.Identity.GetUserId(), false );
            var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );

            if ( _user != null )
            {
                await signInManager.SignInAsync( _user, isPersistent: false, rememberBrowser: false );
            }

            return RedirectToAction( "index", "Manage" );
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber( string phoneNumber )
        {
            var _code = await userManager.GenerateChangePhoneNumberTokenAsync( User.Identity.GetUserId(), phoneNumber );
            // Send an SMS through the SMS provider to verify the phone number

            return phoneNumber == null
                ? View( "Error" )
                : View( new VerifyPhoneNumberViewModel {PhoneNumber = phoneNumber} );
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber( VerifyPhoneNumberViewModel model )
        {
            if( !ModelState.IsValid )
            {
                return View( model );
            }
            var _result =
                await userManager.ChangePhoneNumberAsync( User.Identity.GetUserId(), model.PhoneNumber, model.Code );

            if ( _result.Succeeded )
            {
                var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );
                if( _user != null )
                {
                    await signInManager.SignInAsync( _user, isPersistent: false, rememberBrowser: false );
                }

                return RedirectToAction( "index", new {Message = ManageMessageId.AddPhoneSuccess} );
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError( "", "Failed to verify phone" );

            return View( model );
        }

        //
        // GET: /Manage/RemovePhoneNumber
        public async Task<ActionResult> removePhoneNumber()
        {
            var _result = await userManager.SetPhoneNumberAsync( User.Identity.GetUserId(), null );

            if ( !_result.Succeeded )
            {
                return RedirectToAction( "index", new {Message = ManageMessageId.Error} );
            }
            var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );

            if ( _user != null )
            {
                await signInManager.SignInAsync( _user, isPersistent: false, rememberBrowser: false );
            }

            return RedirectToAction( "index", new {Message = ManageMessageId.RemovePhoneSuccess} );
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword( ChangePasswordViewModel model )
        {
            if( !ModelState.IsValid )
            {
                return View( model );
            }

            var _result =
                await userManager.ChangePasswordAsync( User.Identity.GetUserId(), model.OldPassword, model.NewPassword );

            if ( _result.Succeeded )
            {
                var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );
                if( _user != null )
                {
                    await signInManager.SignInAsync( _user, isPersistent: false, rememberBrowser: false );
                }

                return RedirectToAction( "index", new {Message = ManageMessageId.ChangePasswordSuccess} );
            }
            addErrors( _result );

            return View( model );
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword( SetPasswordViewModel model )
        {
            if( ModelState.IsValid )
            {
                var _result = await userManager.AddPasswordAsync( User.Identity.GetUserId(), model.NewPassword );
                if( _result.Succeeded )
                {
                    var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );
                    if( _user != null )
                    {
                        await signInManager.SignInAsync( _user, isPersistent: false, rememberBrowser: false );
                    }
                    return RedirectToAction( "index", new {Message = ManageMessageId.SetPasswordSuccess} );
                }
                addErrors( _result );
            }

            // If we got this far, something failed, redisplay form
            return View( model );
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> manageLogins( ManageMessageId? message )
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess
                    ? "The external login was removed."
                    : message == ManageMessageId.Error
                        ? "An error has occurred."
                        : "";
            var _user = await userManager.FindByIdAsync( User.Identity.GetUserId() );

            if ( _user == null )
            {
                return View( "Error" );
            }

            var _userLogins = await userManager.GetLoginsAsync( User.Identity.GetUserId() );
            var _otherLogins =
                authenticationManager.GetExternalAuthenticationTypes()
                    .Where( auth => _userLogins.All( ul => auth.AuthenticationType != ul.LoginProvider ) )
                    .ToList();
            ViewBag.ShowRemoveButton = _user.PasswordHash != null || _userLogins.Count > 1;

            return View( new ManageLoginsViewModel
            {
                CurrentLogins = _userLogins,
                OtherLogins = _otherLogins
            } );
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult linkLogin( string provider )
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult( provider, Url.Action( "linkLoginCallback", "Manage" ),
                User.Identity.GetUserId() );
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> linkLoginCallback()
        {
            var _loginInfo = await authenticationManager.GetExternalLoginInfoAsync( XsrfKey, User.Identity.GetUserId() );

            if ( _loginInfo == null )
            {
                return RedirectToAction( "manageLogins", new {Message = ManageMessageId.Error} );
            }
            var _result = await userManager.AddLoginAsync( User.Identity.GetUserId(), _loginInfo.Login );

            return _result.Succeeded
                ? RedirectToAction( "manageLogins" )
                : RedirectToAction( "manageLogins", new {Message = ManageMessageId.Error} );
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing && _userManager != null )
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose( disposing );
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

        private bool hasPassword()
        {
            var _user = userManager.FindById( User.Identity.GetUserId() );

            if ( _user != null )
            {
                return _user.PasswordHash != null;
            }

            return false;
        }

        private bool hasPhoneNumber()
        {
            var _user = userManager.FindById( User.Identity.GetUserId() );

            if ( _user != null )
            {
                return _user.PhoneNumber != null;
            }

            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
        #endregion
    }
}