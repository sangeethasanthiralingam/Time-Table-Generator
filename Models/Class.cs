namespace Time_Table_Generator.Models
{
    public class Class
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Batch> Batches { get; set; } = new List<Batch>();
        public ICollection<Student> Students { get; set; } = new List<Student>(); // Initialize Students to resolve warning
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>(); // Add navigation property for Subjects
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>(); // Add navigation property for Teachers
    }
}
