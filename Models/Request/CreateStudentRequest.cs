namespace Time_Table_Generator.Models.Request
{
    public class CreateStudentRequest
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int BatchId { get; set; }
        public required string RollNumber { get; set; } = string.Empty;
        public required string RegistrationNumber { get; set; } = string.Empty;
    }
}
