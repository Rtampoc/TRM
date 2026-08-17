using Microsoft.EntityFrameworkCore;
using TRM.Data;
using TRM.Models;

namespace TRM.Services
{
    /// <summary>
    /// Authenticates users against the TRMS_Users database table
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user with their username and password
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <param name="password">The user's password</param>
        /// <returns>User object if authentication is successful; null otherwise</returns>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>User object or null if not found</returns>
        Task<User?> GetUserByIdAsync(int userId);
    }

    /// <summary>
    /// Default implementation of authentication service using Entity Framework Core
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(ApplicationDbContext context, ILogger<AuthenticationService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Authenticates a user by querying the TRMS_Users table
        /// </summary>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Authentication attempt with empty username or password");
                return null;
            }

            username = username.Trim();
            password = password.Trim();

            try
            {
                // Query the database for the user (UserId column stores username)
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId.ToLower() == username.ToLower() && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning($"Authentication failed: User '{username}' not found or inactive");
                    return null;
                }

                // Compare passwords (plain-text comparison for now)
                if (!string.Equals(user.Password?.Trim(), password, StringComparison.Ordinal))
                {
                    _logger.LogWarning($"Authentication failed: Invalid password for user '{username}'");
                    return null;
                }

                // Update last login timestamp
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User '{username}' successfully authenticated");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during authentication for user '{username}'");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a user by their ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with ID '{userId}'");
                return null;
            }
        }
    }
}
