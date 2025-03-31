namespace Time_Table_Generator.Models.Request
{
    public class CreateTeacherSubjectRequest
    {
        public required int TeacherId { get; set; }
        public required int SubjectId { get; set; }
        public required int ClassId { get; set; }
    }
}
