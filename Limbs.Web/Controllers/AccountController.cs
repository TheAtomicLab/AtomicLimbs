using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Limbs.Web.Common.Captcha;
using Limbs.Web.Common.Mail;
using Limbs.Web.Resources;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Limbs.Web.Entities.Models;
using Limbs.Web.Entities.WebModels;

namespace Limbs.Web.Controllers
{
    [Authorize]
    [NoCache]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        //
        // GET: /Account/
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectUser();
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectUser();
            }


            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectUser();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Require the user to have a confirmed email before they can log on.
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    await SendEmailConfirmation(user);

                    ViewBag.ErrorMessage = AccountTexts.DisplayEmail_TextInfo;
                    return View("Error");
                }
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("RedirectUser");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
                //case SignInStatus.Failure:
                default:
                    var msgError = @AccountTexts.WrongUserOrPassword;
                    if (string.IsNullOrEmpty(user.PasswordHash))
                        msgError = AccountTexts.FacebookAccount;

                    ModelState.AddModelError("loginfail", msgError);
                    return View(model);
            }
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                    return View("Error");
                default:
                    ModelState.AddModelError("", @"Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectUser();
            }

            return View(new RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateRecaptcha]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectUser();
            }
            if (!ModelState.IsValid) return View(model);

            var userDb = await UserManager.FindByNameAsync(model.Email);
            if (userDb != null && string.IsNullOrEmpty(userDb.PasswordHash))
            {
                ModelState.AddModelError(nameof(model), @AccountTexts.FacebookAccount);
                return View(model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                AddErrors(result);

                // ReSharper disable once InvertIf
                if (IsUserExistError(user, result))
                {
                    var u = await UserManager.FindByNameAsync(model.Email);
                    await SendResetPasswordEmail(u);
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }

            await UserManager.AddToRoleAsync(user.Id, AppRoles.Unassigned);
            await SendEmailConfirmation(user);

            return View("DisplayEmail");
        }

        private bool IsUserExistError(ApplicationUser user, IdentityResult result)
        {
            var os = string.Format(AccountTexts.DuplicateEmail, user.UserName);

            return result.Errors.Any(error => os.Equals(error));
        }

        private async Task SendEmailConfirmation(ApplicationUser user)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            if (Request.Url != null)
            {
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                var body = CompiledTemplateEngine.Render("Mails.EmailConfirmation", callbackUrl);

                await UserManager.SendEmailAsync(user.Id, "[Atomic Limbs] " + AccountTexts.DisplayEmail_ViewBag_Title, body);
            }
        }

        public ActionResult SelectUserOrAmbassador()
        {
            return !User.IsInRole(AppRoles.Unassigned) ? RedirectUser() : View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
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
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", @AccountTexts.UserNotFound);
                return View(model);
            }
            if (!(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                ModelState.AddModelError("", @AccountTexts.DisplayEmail_ViewBag_Title);

                await SendEmailConfirmation(user);

                return View(model);
            }

            if (Request.Url != null)
            {
                await SendResetPasswordEmail(user);
            }
            return RedirectToAction("ForgotPasswordConfirmation", "Account");
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
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
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
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindAsync(loginInfo.Login);

                    if (user == null || await UserManager.IsEmailConfirmedAsync(user.Id))
                        return RedirectToLocal(returnUrl);

                    await SendEmailConfirmation(user);

                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return View("DisplayEmail");

                case SignInStatus.LockedOut:
                    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                //case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

                    return await ExternalLoginConfirmationAction(new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, EmailConfirmed = true }, returnUrl);
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            return await ExternalLoginConfirmationAction(model, returnUrl);
        }

        private async Task<ActionResult> ExternalLoginConfirmationAction(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), AccountTexts.EmailRequired);
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    ModelState.AddModelError(nameof(model), AccountTexts.CantReadFaceInfo);

                    return View("Login");
                }

                var userDb = await UserManager.FindByNameAsync(model.Email);
                if (userDb != null)
                {
                    ModelState.AddModelError(nameof(model), AccountTexts.AccoutNotAssociatedWithFace);
                    return View("Login");
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = model.EmailConfirmed,
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);

                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(user.Id, AppRoles.Unassigned);

                        if (await UserManager.IsEmailConfirmedAsync(user.Id))
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(returnUrl);
                        }

                        await SendEmailConfirmation(user);

                        return View("DisplayEmail");
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginConfirmation", model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }


        public ActionResult RedirectUser()
        {
            if (User.IsInRole(AppRoles.Administrator)) return RedirectToAction("Index", "Panel", new { area = "Admin" });
            if (User.IsInRole(AppRoles.Requester)) return RedirectToAction("Index", "Users");
            if (User.IsInRole(AppRoles.Ambassador)) return RedirectToAction("Index", "Ambassador");
            return RedirectToAction("SelectUserOrAmbassador");
        }

        private async Task SendResetPasswordEmail(ApplicationUser user)
        {
            if (Request == null || Request.Url == null) throw new InvalidOperationException("No request. If you are testing this (thank you :D ), mock que Request and Request.Url.");

            //TODO: token time.
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
            var body = CompiledTemplateEngine.Render("Mails.EmailPasswordChange", callbackUrl);

            await UserManager.SendEmailAsync(user.Id, "[Atomic Limbs] " + AccountTexts.ForgotPassword_SubmitButton, body);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

    /// <inheritdoc />
    /// <summary>
    /// Prevent a controller or specific action from being cached in the web browser.
    /// For example - sign in, go to a secure page, sign out, click the back button.
    /// <seealso cref="!:https://stackoverflow.com/questions/6656476/mvc-back-button-issue/6656539#6656539" />
    /// </summary>
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            response.Cache.SetValidUntilExpires(false);
            response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
        }
    }
}