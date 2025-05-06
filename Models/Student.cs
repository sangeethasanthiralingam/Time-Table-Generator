namespace Time_Table_Generator.Models
{
    public class Student
    {
        public int Id { get; set; }
        public required int UserId { get; set; } 
        public User? User { get; set; } 
        public required int BatchId { get; set; }
        public Batch? Batch { get; set; }
        public required string RollNumber { get; set; } = string.Empty;
        public required string RegistrationNumber { get; set; } = string.Empty;
        public int ClassId { get; set; } // Foreign key for Class
    }
}