namespace OlsaMarketing.Models
{
    public class ReportView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<TaskView> Tasks { get; set; } = new List<TaskView>();
    }

}

