namespace Time_Table_Generator.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public required string Type { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Date { get; set; }
    }
}