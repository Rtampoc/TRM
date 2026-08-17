namespace TRM.Models
{
    /// <summary>
    /// User model representing a record in the TRMS_Users table
    /// </summary>
    public class User
    {
        // Primary key in database
        public int Id { get; set; }

        // Username stored in database column 'UserId' (varchar)
        public string UserId { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string? EmployeeName { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? DateCreated { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
