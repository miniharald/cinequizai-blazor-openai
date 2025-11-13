using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CineQuizAI.Infrastructure.Identity;
using Microsoft.Extensions.Logging;

namespace CineQuizAI.Web.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // Only POST handles /auth/login2 to avoid route conflict with Blazor page
        [HttpPost("login2")]
        [ValidateAntiForgeryToken] // Re-enabled for CSRF protection
        public async Task<IActionResult> Login(string userNameOrEmail, string password, string? returnUrl)
        {
            var user = await _userManager.FindByNameAsync(userNameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(userNameOrEmail);
            if (user == null)
                return RedirectToLoginWithError(returnUrl, "InvalidCredentials");

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? string.Empty, password, true, false);
            if (!result.Succeeded)
                return RedirectToLoginWithError(returnUrl, "InvalidCredentials");

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return Redirect("/welcome");
        }

        [HttpPost("register2")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string userName, string email, string password, string confirmPassword, string? returnUrl)
        {
            try
            {
                _logger.LogInformation("Register attempt for username: {UserName}, email: {Email}", userName, email);

                // Validate input
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    _logger.LogWarning("Registration failed: Missing required fields");
                    return RedirectToRegisterWithError(returnUrl, "MissingFields", userName, email);
                }

                // Validate passwords match
                if (password != confirmPassword)
                {
                    _logger.LogWarning("Registration failed: Passwords do not match");
                    return RedirectToRegisterWithError(returnUrl, "PasswordsDoNotMatch", userName, email);
                }

                // Check if username is taken
                if (await _userManager.FindByNameAsync(userName) is not null)
                {
                    _logger.LogWarning("Registration failed: Username {UserName} already taken", userName);
                    return RedirectToRegisterWithError(returnUrl, "UsernameTaken", userName, email);
                }

                // Check if email is in use
                if (await _userManager.FindByEmailAsync(email) is not null)
                {
                    _logger.LogWarning("Registration failed: Email {Email} already in use", email);
                    return RedirectToRegisterWithError(returnUrl, "EmailInUse", userName, email);
                }

                // Create user
                var user = new AppUser
                {
                    UserName = userName,
                    Email = email
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("User creation failed: {Errors}", errors);
                    return RedirectToRegisterWithError(returnUrl, "CreationFailed", userName, email);
                }

                _logger.LogInformation("User {UserName} created successfully", userName);

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: true);
                _logger.LogInformation("User {UserName} signed in successfully", userName);

                // Redirect to ReturnUrl if present, otherwise to /welcome
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return Redirect("/welcome");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                return RedirectToRegisterWithError(returnUrl, "UnexpectedError", userName, email);
            }
        }

        private IActionResult RedirectToLoginWithError(string? returnUrl, string error)
        {
            var url = "/auth/login?error=" + error;
            if (!string.IsNullOrWhiteSpace(returnUrl))
                url += "&ReturnUrl=" + Uri.EscapeDataString(returnUrl);
            return Redirect(url);
        }

        private IActionResult RedirectToRegisterWithError(string? returnUrl, string error, string? userName = null, string? email = null)
        {
            var url = "/auth/register?error=" + error;

            // Only preserve non-sensitive data (NEVER passwords!)
            if (!string.IsNullOrWhiteSpace(userName))
                url += "&userName=" + Uri.EscapeDataString(userName);
            if (!string.IsNullOrWhiteSpace(email))
                url += "&email=" + Uri.EscapeDataString(email);

            if (!string.IsNullOrWhiteSpace(returnUrl))
                url += "&returnUrl=" + Uri.EscapeDataString(returnUrl);

            return Redirect(url);
        }

        [HttpGet("signout")]
        [HttpPost("signout")]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User signed out");
            return Redirect("/signed-out");
        }
    }
}
