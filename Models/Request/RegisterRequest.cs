namespace Time_Table_Generator.Models.Request
{
    public class RegisterRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Displayname { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public UserRole? Role { get; set; }
        public UserType? UserType { get; set; }
        
        // Student-specific properties
        public string? RollNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? BatchId { get; set; }
    }
}