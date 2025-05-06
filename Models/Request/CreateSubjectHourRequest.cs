namespace Time_Table_Generator.Models.Request
{
    public class CreateSubjectHourRequest
    {
        public required int SubjectId { get; set; }
        public required int HoursInWeek { get; set; }
        public required int HoursInDay { get; set; }
    }
}
