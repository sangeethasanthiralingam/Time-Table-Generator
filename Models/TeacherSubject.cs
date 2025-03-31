namespace Time_Table_Generator.Models
{
    public class TeacherSubject
    {
        public int Id { get; set; }

        // Foreign Key for Teacher
        public required int TeacherId { get; set; }
        public Teacher? Teacher { get; set; } 

        // Foreign Key for Subject
        public required int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        // Foreign Key for Class
        public required int ClassId { get; set; }
        public Class? Class { get; set; }
    }
}
