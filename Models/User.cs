namespace Time_Table_Generator.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public required string Displayname { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public required UserType UserType { get; set; } = UserType.Student;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public required string Phone { get; set; } = string.Empty;
        public required string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string? Token { get; set; }
    }
}

public enum UserRole { Admin, User, Guest }
public enum UserType { Student, Teacher, Staff }
public enum UserStatus { Active, Inactive, Suspended, Deleted }