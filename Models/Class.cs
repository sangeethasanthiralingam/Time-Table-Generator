namespace Time_Table_Generator.Models
{
    public class Class
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required int BatchId { get; set; }
        public Batch? Batch { get; set; }
    }
}