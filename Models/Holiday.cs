namespace Time_Table_Generator.Models
{
    public class Holiday
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required string Description { get; set; } = string.Empty;
        public required string Type { get; set; } = string.Empty;
        public required DateTime Date { get; set; }
    }
}