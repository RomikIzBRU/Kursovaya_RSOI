namespace OlsaMarketing.Models
{
    public class CampaignView
    {
        public int Id { get; set; }
        public string CampaignName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; }
    }
}
