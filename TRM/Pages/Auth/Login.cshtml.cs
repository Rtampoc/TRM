using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TRM.Services;

namespace TRM.Pages.Auth
{
    /// <summary>
    /// Login page model handling authentication and session setup
    /// </summary>
    public class LoginModel : PageModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public LoginViewModel? LoginForm { get; set; }

        public LoginModel(IAuthenticationService authenticationService, ILogger<LoginModel> logger)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            LoginForm = new LoginViewModel();
        }

        /// <summary>
        /// Handles GET requests - initializes the login form
        /// If already authenticated, redirect to the main page.
        /// </summary>
        public IActionResult OnGet()
        {
            // Check if user is already logged in
            var existingUserId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(existingUserId))
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        /// <summary>
        /// Handles POST requests - authenticates user and creates session
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoginForm ??= new LoginViewModel();
                return Page();
            }

            // Initial validation
            if (string.IsNullOrWhiteSpace(LoginForm?.UserId) || string.IsNullOrWhiteSpace(LoginForm?.Password))
            {
                LoginForm ??= new LoginViewModel();
                LoginForm.ErrorMessage = "User ID and Password are required.";
                return Page();
            }

            var username = LoginForm.UserId.Trim();
            var password = LoginForm.Password.Trim();

            // Authenticate using the AuthenticationService
            var user = await _authenticationService.AuthenticateAsync(username, password);

            if (user != null)
            {
                // Set authentication session variables
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.UserId ?? "");
                HttpContext.Session.SetString("EmployeeName", user.EmployeeName ?? "");
                HttpContext.Session.SetString("Department", user.Department ?? "");
                HttpContext.Session.SetString("Email", user.Email ?? "");
                HttpContext.Session.SetString("Role", user.Role ?? "");

                if (LoginForm.RememberMe)
                {
                    // Create a persistent cookie for remember me functionality
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };
                    Response.Cookies.Append("RememberedUserId", user.Id.ToString(), cookieOptions);
                }

                _logger.LogInformation($"User '{user.UserId}' successfully logged in from {HttpContext.Connection.RemoteIpAddress}");

                // Redirect to dashboard after successful authentication
                return RedirectToPage("/Index");
            }

            // Authentication failed
            LoginForm.ErrorMessage = "Invalid User ID or Password. Please try again.";
            LoginForm.Password = string.Empty; // Clear password for security
            _logger.LogWarning($"Failed login attempt for user '{LoginForm.UserId}' from {HttpContext.Connection.RemoteIpAddress}");
            return Page();
        }
    }

    /// <summary>
    /// Login view model for form binding
    /// </summary>
    public class LoginViewModel
    {
        public string? UserId { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
