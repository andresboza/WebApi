namespace WebApi.DTOs.Filters
{
    public class PositionFilter
    {
        public string? Status { get; set; }
        public string? Location { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
