namespace Time_Table_Generator.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Class> Classes { get; set; } = new List<Class>(); // Add navigation property for Classes
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>(); // Add navigation property for Teachers
    }
}