namespace Time_Table_Generator.Models
{
    public class EventClassBatch
    {
        public int Id { get; set; }
        public required int EventId { get; set; }
        public required EventModel Event { get; set; } = null!;
        public required int BatchId { get; set; }
        public required Batch Batch { get; set; } = null!;
        public required int ClassId { get; set; }
        public required Class Class { get; set; } = null!;

        // Ensure no cascading delete behavior
        // Relationships will be configured in DataContext
    }
}