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

        public List<User> Users { get; set; } = new();

        public UserStatistics Statistics { get; set; } = new();

        public List<string> Departments { get; set; } = new();

        public string SearchQuery { get; set; } = string.Empty;

        public string RoleFilter { get; set; } = "All Roles";

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
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check authorization - only admins can access
                // Session stores role in key "Role" (set during login). Use that key.
                var userRole = HttpContext.Session.GetString("Role");
                if (userRole != "Admin" && userRole != "admin")
                {
                    _logger.LogWarning($"Unauthorized access attempt to User Management from role: {userRole}");
                    return RedirectToPage("/Index");
                }

                await LoadUserData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user management page");
                ModelState.AddModelError("", "An error occurred while loading user data.");
            }

            return Page();
        }

        /// <summary>
        /// Handles creating a new user
        /// </summary>
        public async Task<IActionResult> OnPostCreateAsync()
        {
            try
            {
                // Check authorization
                var userRole = HttpContext.Session.GetString("Role");
                if (userRole != "Admin" && userRole != "admin")
                {
                    return Forbid();
                }

                NewUser.UserId = (NewUser.UserId ?? string.Empty).Trim();
                NewUser.EmployeeName = (NewUser.EmployeeName ?? string.Empty).Trim();
                NewUser.Email = (NewUser.Email ?? string.Empty).Trim();
                NewUser.Department = (NewUser.Department ?? string.Empty).Trim();
                NewUser.Role = (NewUser.Role ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(NewUser.UserId))
                    ModelState.AddModelError("NewUser.UserId", "User ID is required.");
                else if (NewUser.UserId.Length > 50)
                    ModelState.AddModelError("NewUser.UserId", "User ID cannot exceed 50 characters.");

                if (string.IsNullOrWhiteSpace(NewUser.EmployeeName))
                    ModelState.AddModelError("NewUser.EmployeeName", "Full name is required.");
                else if (NewUser.EmployeeName.Length > 100)
                    ModelState.AddModelError("NewUser.EmployeeName", "Full name cannot exceed 100 characters.");

                if (string.IsNullOrWhiteSpace(NewUser.Password))
                    ModelState.AddModelError("NewUser.Password", "Password is required.");
                else if (NewUser.Password.Length > 255)
                    ModelState.AddModelError("NewUser.Password", "Password cannot exceed 255 characters.");

                if (string.IsNullOrWhiteSpace(NewUser.Email))
                    ModelState.AddModelError("NewUser.Email", "Email is required.");
                else if (NewUser.Email.Length > 255)
                    ModelState.AddModelError("NewUser.Email", "Email cannot exceed 255 characters.");
                else if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(NewUser.Email))
                    ModelState.AddModelError("NewUser.Email", "Enter a valid email address.");

                if (string.IsNullOrWhiteSpace(NewUser.Department))
                    ModelState.AddModelError("NewUser.Department", "Department is required.");
                else if (NewUser.Department.Length > 100)
                    ModelState.AddModelError("NewUser.Department", "Department cannot exceed 100 characters.");

                NewUser.Role = NewUser.Role.ToLowerInvariant() switch
                {
                    "admin" => "Admin",
                    "approver" => "Approver",
                    "encoder" => "Encoder",
                    "viewer" => "Viewer",
                    _ => string.Empty
                };
                if (string.IsNullOrEmpty(NewUser.Role))
                    ModelState.AddModelError("NewUser.Role", "Select an access level.");

                if (!ModelState.IsValid)
                {
                    ViewData["OpenAddModal"] = true;
                    await LoadUserData();
                    return Page();
                }

                // Create user
                var createdUser = await _userManagementService.CreateUserAsync(NewUser);

                _logger.LogInformation($"User '{createdUser.UserId}' created successfully");

                TempData["SuccessMessage"] = $"User '{createdUser.EmployeeName}' created successfully!";
                return RedirectToPage();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User creation was rejected for user ID '{UserId}'", NewUser.UserId);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with ID '{UserId}'", NewUser.UserId);
                TempData["ErrorMessage"] = "The user could not be saved. Please try again or contact support if the problem continues.";
                return RedirectToPage();
            }
        }

        /// <summary>
        /// Handles updating a user
        /// </summary>
        public async Task<IActionResult> OnPostUpdateAsync(int userId)
        {
            try
            {
                // Check authorization
                var userRole = HttpContext.Session.GetString("Role");
                if (userRole != "Admin" && userRole != "admin")
                {
                    return Forbid();
                }

                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var wasActive = user.IsActive;
                if (!NewUser.IsActive && wasActive &&
                    int.TryParse(HttpContext.Session.GetString("UserId"), out var currentUserId) &&
                    user.Id == currentUserId)
                {
                    TempData["ErrorMessage"] = "You cannot deactivate your own account.";
                    return RedirectToPage();
                }

                // Update user properties from request
                if (!string.IsNullOrWhiteSpace(NewUser.EmployeeName))
                    user.EmployeeName = NewUser.EmployeeName;

                if (!string.IsNullOrWhiteSpace(NewUser.Email))
                    user.Email = NewUser.Email;

                if (!string.IsNullOrWhiteSpace(NewUser.Department))
                    user.Department = NewUser.Department.Trim();

                if (!string.IsNullOrWhiteSpace(NewUser.Role))
                    user.Role = NewUser.Role;

                if (!string.IsNullOrWhiteSpace(NewUser.Password))
                    user.Password = NewUser.Password;

                user.IsActive = NewUser.IsActive;

                await _userManagementService.UpdateUserAsync(user);

                _logger.LogInformation("User {UserId} updated successfully", user.UserId);

                await LoadUserData();
                TempData["SuccessMessage"] = wasActive != user.IsActive
                    ? $"User '{user.EmployeeName}' account {(user.IsActive ? "activated" : "deactivated")}."
                    : $"User '{user.EmployeeName}' updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                ModelState.AddModelError("", "An error occurred while updating the user.");
                // Keep edit modal open and preserve the entered values
                ViewData["OpenEditModal"] = true;
                ViewData["EditUserId"] = userId;
                await LoadUserData();
            }

            return Page();
        }

        /// <summary>
        /// Handles toggling user active status
        /// </summary>
        public async Task<IActionResult> OnPostToggleStatusAsync(int userId, bool? isActive)
        {
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Admin" && userRole != "admin")
            {
                return Forbid();
            }

            if (userId <= 0 || !isActive.HasValue)
            {
                return BadRequest();
            }

            try
            {
                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                if (!isActive.Value && user.IsActive &&
                    int.TryParse(HttpContext.Session.GetString("UserId"), out var currentUserId) &&
                    user.Id == currentUserId)
                {
                    TempData["ErrorMessage"] = "You cannot deactivate your own account.";
                    return RedirectToPage();
                }

                if (user.IsActive != isActive.Value)
                {
                    var updated = isActive.Value
                        ? await _userManagementService.ActivateUserAsync(userId)
                        : await _userManagementService.DeactivateUserAsync(userId);

                    if (!updated)
                    {
                        return NotFound();
                    }
                }

                TempData["SuccessMessage"] = isActive.Value
                    ? $"User '{user.EmployeeName}' activated."
                    : $"User '{user.EmployeeName}' deactivated.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting active status for user ID {UserId}", userId);
                TempData["ErrorMessage"] = "The account status could not be changed. Please try again.";
            }

            return RedirectToPage();
        }

        /// <summary>
        /// Handles deleting a user
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            try
            {
                // Check authorization
                var userRole = HttpContext.Session.GetString("Role");
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
                // Get statistics and department options
                Statistics = await _userManagementService.GetUserStatisticsAsync();
                Departments = await _userManagementService.GetDepartmentsAsync();

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
