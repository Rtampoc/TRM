using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TRM.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnGet()
        {
            // Get user info for logging
            var userId = HttpContext.Session.GetString("UserId");
            var username = HttpContext.Session.GetString("Username");

            // Clear session
            HttpContext.Session.Clear();

            // Clear authentication cookies
            Response.Cookies.Delete("RememberedUserId");

            // Log logout action
            _logger.LogInformation($"User '{username}' (ID: {userId}) logged out from {HttpContext.Connection.RemoteIpAddress}");

            // Note: The redirect to login page is handled by the JavaScript in the view
        }
    }
}
