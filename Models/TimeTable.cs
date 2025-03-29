namespace Time_Table_Generator.Models
{
    public class TimeTable
    {
        public int Id { get; set; }

        // Foreign Key for Class
        public required int ClassId { get; set; }
        public Class? Class { get; set; }

        // Foreign Key for Batch
        public required int BatchId { get; set; }
        public Batch? Batch { get; set; }

        // Foreign Key for Teacher
        public required int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // Foreign Key for Subject
        public required int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public required DateTime Date { get; set; }
    }
}
