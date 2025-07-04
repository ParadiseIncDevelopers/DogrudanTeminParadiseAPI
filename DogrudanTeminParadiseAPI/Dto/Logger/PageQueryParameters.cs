namespace DogrudanTeminParadiseAPI.Dto.Logger
{
    public class PageQueryParameters
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 500;
        public string? PageUrl { get; set; }
    }
}
