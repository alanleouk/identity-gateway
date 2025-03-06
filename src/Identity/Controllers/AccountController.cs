using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Identity.Repository;
using Identity.Models.AccountModels;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Identity.Constants;
using Identity.Models.FindModels;
using Identity.Models.Responses;
using Identity.Services;
using Microsoft.AspNetCore.Authentication;

namespace Identity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IdentityDbContext _db;
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly ILogger _logger;

        public AccountController(
            IdentityDbContext db,
            IUserService userService,
            ILoginService loginService,
            ILoggerFactory loggerFactory)
        {
            _db = db;
            _userService = userService;
            _loginService = loginService;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            return await LogOff();
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _loginService.LogoutAsync();
            await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult CodeLogin(string? returnUrl = null)
        {
            // TODO: Code Login
            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(Login));
        }
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null, string? publicKey = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties =
                _loginService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            var result = Challenge(properties, provider);
            return result;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _loginService.GetUserExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result =
                await _loginService.ExternalLoginSignInAsync(info, true);
            
            if (result.Success)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider key '{Key}'.", info.LoginProvider,
                    info.ProviderKey);
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationModel model,
            string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _loginService.GetUserExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var user = new User { Username = model.Email, Email = model.Email };
                var result = await _userService.CreateAsync(user);
                if (result.Success)
                {
                    result = await _userService.AddExternalLoginAsync(user.Id, info);
                    if (result.Success)
                    {
                        await _loginService.LoginAsync(user, false, null);
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Info()
        {
            var model = new InfoModel();

            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var certificates = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName,
                "CN=identity-signing", false);
            if ((certificates?.Count ?? 0) > 0)
            {
                model.Certificates = certificates[0].Thumbprint;
            }

            var user = await _userService.FindAsync(new ByClaimsPrincipal(HttpContext.User));
            if (user != null)
            {
                var schema = await _loginService.GetExternalAuthenticationSchemesAsync();
                var otherLogins = schema.ToList();
                model.OtherLogins = otherLogins;

                var logins = await _userService.GeTUserExternalLoginsAsync(user);
                model.Logins = logins.Select(item => new LoginProviderModel
                {
                    LoginProvider = item.Provider,
                    ProviderDisplayName = item.Name
                }).ToList();

                model.Email = user.Email;
                model.Roles = string.Join(", ", await _userService.GetRolesAsync(user.Id));
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super")]
        public async Task<ActionResult> ImportCertificate(string password)
        {
            var file = Request.Form.Files[0];

            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);

            var certificates = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName,
                "CN=identity-signing", false);
            store.RemoveRange(certificates);

            using (var stream = file.OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    store.Add(new X509Certificate2(ms.ToArray(), password));
                }
            }

            return RedirectToAction(nameof(Info), new { Message = "Certificate Imported" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Account");
            var userId = _userService.GetUserId(User);
            if (userId == null)
            {
                throw new ArgumentException("User cannot be null");
            }

            var properties = _loginService.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await _userService.FindAsync(new ByClaimsPrincipal(HttpContext.User));
            if (user == null)
            {
                return View("Error");
            }

            var info = await _loginService.GetUserExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                return RedirectToAction(nameof(Info), new { Message = ManageMessageId.Error });
            }

            var result = await _userService.AddExternalLoginAsync(user.Id, info);
            var message = result.Success ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(Info), new { Message = message });
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private void AddErrors(IErrorResponse response)
        {
            foreach (var error in response.Errors)
            {
                ModelState.AddModelError(error.ErrorCode, error.ErrorMessage);
            }
        }

        private Task<User?> GetCurrentUserAsync()
        {
            return _userService.FindAsync(new ByClaimsPrincipal(HttpContext.User));
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
