namespace Time_Table_Generator.Models
{
    public class Batch
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ClassId { get; set; }
        public virtual Class? Class { get; set; }
        public ICollection<Student> Students { get; set; } = new List<Student>(); // Add navigation property for Students
    }
}
