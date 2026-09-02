using Microsoft.EntityFrameworkCore;
using TRM.Data;
using TRM.Models;

namespace TRM.Services
{
    /// <summary>
    /// Service for managing application users and roles
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Gets all users with optional filtering
        /// </summary>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Gets user statistics for dashboard
        /// </summary>
        Task<UserStatistics> GetUserStatisticsAsync();

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        Task<User?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Creates a new user
        /// </summary>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user
        /// </summary>
        Task<bool> DeleteUserAsync(int userId);

        /// <summary>
        /// Gets users by role
        /// </summary>
        Task<List<User>> GetUsersByRoleAsync(string role);

        /// <summary>
        /// Deactivates a user
        /// </summary>
        Task<bool> DeactivateUserAsync(int userId);

        /// <summary>
        /// Activates a user
        /// </summary>
        Task<bool> ActivateUserAsync(int userId);
    }

    /// <summary>
    /// Default implementation of user management service
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(ApplicationDbContext context, ILogger<UserManagementService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all users sorted by creation date
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users
                    .OrderByDescending(u => u.DateCreated)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        /// <summary>
        /// Gets user statistics for the dashboard
        /// </summary>
        public async Task<UserStatistics> GetUserStatisticsAsync()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                return new UserStatistics
                {
                    TotalUsers = users.Count,
                    ActiveUsers = users.Count(u => u.IsActive),
                    InactiveUsers = users.Count(u => !u.IsActive),
                    AdminUsers = users.Count(u => u.Role == "Admin" || u.Role == "admin")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user statistics");
                throw;
            }
        }

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with ID {userId}");
                throw;
            }
        }

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrWhiteSpace(user.UserId))
                    throw new ArgumentException("Username is required");

                // Check if user already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
                if (existingUser != null)
                    throw new InvalidOperationException($"User with username '{user.UserId}' already exists");

                user.DateCreated = DateTime.UtcNow;
                user.IsActive = true;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{user.UserId}' created successfully");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existingUser == null)
                    throw new InvalidOperationException($"User with ID {user.Id} not found");

                existingUser.EmployeeName = user.EmployeeName;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;
                existingUser.IsActive = user.IsActive;
                existingUser.Department = user.Department;

                // Only update password if provided
                if (!string.IsNullOrWhiteSpace(user.Password))
                    existingUser.Password = user.Password;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{existingUser.UserId}' updated successfully");
                return existingUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                throw;
            }
        }

        /// <summary>
        /// Deletes a user from the database
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User with ID {userId} deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                throw;
            }
        }

        /// <summary>
        /// Gets users by role
        /// </summary>
        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException("Role is required");

                return await _context.Users
                    .Where(u => u.Role == role || u.Role == role.ToLower())
                    .OrderByDescending(u => u.DateCreated)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving users with role '{role}'");
                throw;
            }
        }

        /// <summary>
        /// Deactivates a user
        /// </summary>
        public async Task<bool> DeactivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                user.IsActive = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User with ID {userId} deactivated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user");
                throw;
            }
        }

        /// <summary>
        /// Activates a user
        /// </summary>
        public async Task<bool> ActivateUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return false;

                user.IsActive = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User with ID {userId} activated");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                throw;
            }
        }
    }

    /// <summary>
    /// Data transfer object for user statistics
    /// </summary>
    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int AdminUsers { get; set; }
    }
}
