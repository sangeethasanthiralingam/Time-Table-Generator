namespace Time_Table_Generator.Models
{
    public class EventModel 
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public required DateTime Date { get; set; }
    }
}