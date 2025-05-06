namespace Time_Table_Generator.Models.Request
{
    public class CreateBatchRequest
    {
        public required string Name { get; set; }
        public int ClassId { get; set; }
    }
}
