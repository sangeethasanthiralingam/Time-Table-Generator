namespace Time_Table_Generator.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Class> Classes { get; set; } = new List<Class>(); // Add navigation property for Classes
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>(); // Add navigation property for Subjects
    }
}
