using System.ComponentModel.DataAnnotations;

namespace OlsaMarketing.Models
{
    public class TaskView
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int CampaignId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime DueTo { get; set; }

        public int ReportId { get; set; }

        public Report Report { get; set; }

        public Employee Employee { get; set; }
        public Campaign Campaign { get; set; }
        public List<string> Photos { get; set; } = new List<string>();
    }
}

