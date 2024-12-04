namespace OlsaMarketing.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }


}
