namespace Time_Table_Generator.Models
{
    public class SubjectHour
    {
        public int Id { get; set; }
        public required int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public required int HoursInWeek { get; set; }
        public required int HoursInDay { get; set; }
    }
}