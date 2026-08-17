using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TRM.Pages
{
    public class IndexModel : PageModel
    {
        public List<ToolingRequestSummary>? ToolingRequests { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }

        public void OnGet()
        {
            // Check if user is authenticated
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                RedirectToPage("/Auth/Login");
                return;
            }

            // Load user information from session
            UserName = HttpContext.Session.GetString("EmployeeName") ?? "User";
            UserEmail = HttpContext.Session.GetString("Email") ?? string.Empty;

            // TODO: Load tooling requests from database
            // For now, showing sample data
            ToolingRequests = new List<ToolingRequestSummary>();

            // Sample data - replace with actual database query
            ToolingRequests.Add(new ToolingRequestSummary
            {
                Id = 1,
                RequestNumber = "TRF-2026-001",
                CustomerName = "3C Signature Asian Group Corp",
                Status = "DRAFT",
                CreatedDate = DateTime.Now.AddDays(-5)
            });

            ToolingRequests.Add(new ToolingRequestSummary
            {
                Id = 2,
                RequestNumber = "TRF-2026-002",
                CustomerName = "TechCorp Manufacturing",
                Status = "SUBMITTED",
                CreatedDate = DateTime.Now.AddDays(-3)
            });
        }
    }

    /// <summary>
    /// Summary model for displaying tooling requests in the list
    /// </summary>
    public class ToolingRequestSummary
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
