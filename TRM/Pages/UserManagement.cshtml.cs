using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TRM.Models;
using TRM.Services;

namespace TRM.Pages
{
    /// <summary>
    /// Page model for User Management dashboard
    /// </summary>
    public class UserManagementModel : PageModel
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UserManagementModel> _logger;

        [BindProperty]
        public List<User> Users { get; set; } = new();

        [BindProperty]
        public UserStatistics Statistics { get; set; } = new();

        [BindProperty]
        public string SearchQuery { get; set; } = string.Empty;

        [BindProperty]
        public string RoleFilter { get; set; } = "All Roles";

        [BindProperty]
        public string StatusFilter { get; set; } = "All Status";

        [BindProperty]
        public User NewUser { get; set; } = new();

        public UserManagementModel(IUserManagementService userManagementService, ILogger<UserManagementModel> logger)
        {
            _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                // Check authorization - only admins can access
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin" && userRole != "admin")
                {
                    _logger.LogWarning($"Unauthorized access attempt to User Management from role: {userRole}");
                    RedirectToPage("/Index");
                    return;
                }

                await LoadUserData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user management page");
                ModelState.AddModelError("", "An error occurred while loading user data.");
            }
        }

        /// <summary>
        /// Handles creating a new user
        /// </summary>
        public async Task<IActionResult> OnPostCreateAsync()
        {
            try
            {
                // Check authorization
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin" && userRole != "admin")
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    await LoadUserData();
                    return Page();
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(NewUser.UserId) ||
                    string.IsNullOrWhiteSpace(NewUser.Password) ||
                    string.IsNullOrWhiteSpace(NewUser.EmployeeName) ||
                    string.IsNullOrWhiteSpace(NewUser.Email) ||
                    string.IsNullOrWhiteSpace(NewUser.Role))
                {
                    ModelState.AddModelError("", "All fields are required");
                    await LoadUserData();
                    return Page();
                }

                // Create user
                var createdUser = await _userManagementService.CreateUserAsync(NewUser);

                _logger.LogInformation($"User '{createdUser.UserId}' created successfully");

                // Reload data
                await LoadUserData();
                NewUser = new(); // Reset form

                TempData["SuccessMessage"] = $"User '{createdUser.EmployeeName}' created successfully!";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadUserData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                ModelState.AddModelError("", "An error occurred while creating the user.");
                await LoadUserData();
            }

            return Page();
        }

        /// <summary>
        /// Handles updating a user
        /// </summary>
        public async Task<IActionResult> OnPostUpdateAsync(int userId)
        {
            try
            {
                // Check authorization
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin" && userRole != "admin")
                {
                    return Forbid();
                }

                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user properties from request
                if (!string.IsNullOrWhiteSpace(NewUser.EmployeeName))
                    user.EmployeeName = NewUser.EmployeeName;

                if (!string.IsNullOrWhiteSpace(NewUser.Email))
                    user.Email = NewUser.Email;

                if (!string.IsNullOrWhiteSpace(NewUser.Role))
                    user.Role = NewUser.Role;

                if (!string.IsNullOrWhiteSpace(NewUser.Password))
                    user.Password = NewUser.Password;

                user.IsActive = NewUser.IsActive;

                await _userManagementService.UpdateUserAsync(user);

                _logger.LogInformation($"User '{user.UserId}' updated successfully");

                await LoadUserData();
                TempData["SuccessMessage"] = $"User '{user.EmployeeName}' updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                ModelState.AddModelError("", "An error occurred while updating the user.");
                await LoadUserData();
            }

            return Page();
        }

        /// <summary>
        /// Handles toggling user active status
        /// </summary>
        public async Task<IActionResult> OnPostToggleStatusAsync(int userId)
        {
            try
            {
                // Check authorization
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin" && userRole != "admin")
                {
                    return Forbid();
                }

                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                if (user.IsActive)
                {
                    await _userManagementService.DeactivateUserAsync(userId);
                    TempData["SuccessMessage"] = $"User '{user.EmployeeName}' deactivated.";
                }
                else
                {
                    await _userManagementService.ActivateUserAsync(userId);
                    TempData["SuccessMessage"] = $"User '{user.EmployeeName}' activated.";
                }

                await LoadUserData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status");
                ModelState.AddModelError("", "An error occurred while toggling user status.");
                await LoadUserData();
            }

            return Page();
        }

        /// <summary>
        /// Handles deleting a user
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            try
            {
                // Check authorization
                var userRole = HttpContext.Session.GetString("UserRole");
                if (userRole != "Admin" && userRole != "admin")
                {
                    return Forbid();
                }

                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var userName = user.EmployeeName;
                await _userManagementService.DeleteUserAsync(userId);

                _logger.LogInformation($"User with ID {userId} deleted");

                await LoadUserData();
                TempData["SuccessMessage"] = $"User '{userName}' deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                ModelState.AddModelError("", "An error occurred while deleting the user.");
                await LoadUserData();
            }

            return Page();
        }

        /// <summary>
        /// Helper method to load user data with filtering
        /// </summary>
        private async Task LoadUserData()
        {
            try
            {
                // Get statistics
                Statistics = await _userManagementService.GetUserStatisticsAsync();

                // Get all users
                var allUsers = await _userManagementService.GetAllUsersAsync();

                // Apply filters
                var filtered = allUsers;

                // Filter by search query
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    var searchLower = SearchQuery.ToLower();
                    filtered = filtered
                        .Where(u => u.EmployeeName?.ToLower().Contains(searchLower) == true ||
                                   u.Email?.ToLower().Contains(searchLower) == true ||
                                   u.UserId?.ToLower().Contains(searchLower) == true)
                        .ToList();
                }

                // Filter by role
                if (!string.IsNullOrWhiteSpace(RoleFilter) && RoleFilter != "All Roles")
                {
                    filtered = filtered.Where(u => u.Role == RoleFilter).ToList();
                }

                // Filter by status
                if (!string.IsNullOrWhiteSpace(StatusFilter))
                {
                    if (StatusFilter == "Active")
                        filtered = filtered.Where(u => u.IsActive).ToList();
                    else if (StatusFilter == "Inactive")
                        filtered = filtered.Where(u => !u.IsActive).ToList();
                }

                Users = filtered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user data");
                throw;
            }
        }
    }
}
