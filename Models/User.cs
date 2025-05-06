using System.Security.Cryptography;
using System.Text;

namespace Time_Table_Generator.Models
{
    public class User
    {
        public int Id { get; set; }
        
        // User's first and last names can be empty strings
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        // Displayname, Phone, Address, Email, and Password are required fields
        public required string Displayname { get; set; } = string.Empty;
        public required string Phone { get; set; } = string.Empty;
        public required string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Password field should be hashed before storage
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User; // Default role is 'User'
        public required UserType UserType { get; set; } = UserType.Student; // Default type is 'Student'
        public UserStatus Status { get; set; } = UserStatus.Active; // Default status is 'Active'

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Created time
        public DateTime UpdatedAt { get; set; } = DateTime.Now; // Updated time

        public string? Token { get; set; } // JWT token after login
    }

    // Enum for User Roles
    public enum UserRole { Admin, User, Guest }

    // Enum for User Types
    public enum UserType { Student, Teacher, Staff }

    // Enum for User Status
    public enum UserStatus { Active, Inactive, Suspended, Deleted }

    // Helper method to hash the password before saving it in the database
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
