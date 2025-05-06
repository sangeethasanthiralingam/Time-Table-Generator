namespace Time_Table_Generator.Models.Request
{
    public class CreateClassTeacherRequest
    { 
        public required int TeacherId { get; set; }
        public required int ClassId { get; set; }
        public required int BatchId { get; set; }
    }
}
