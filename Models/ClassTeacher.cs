namespace Time_Table_Generator.Models
{
    public class ClassTeacher
    {
        public int Id { get; set; }
        
        // Foreign Key for Teacher
        public required int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }  

        // Foreign Key for Class
        public required int ClassId { get; set; }
        public Class? Class { get; set; }

        // Foreign Key for Batch
        public required int BatchId { get; set; }
        public Batch? Batch { get; set; }  
    }
}
