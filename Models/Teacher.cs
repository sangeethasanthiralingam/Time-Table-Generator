namespace Time_Table_Generator.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public User? User { get; set; }
    }
}
