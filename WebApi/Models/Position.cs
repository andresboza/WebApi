namespace WebApi.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "draft"; // draft/open/closed/archived
        public int RecruiterId { get; set; }
        public int DepartmentId { get; set; }
        public decimal Budget { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}
