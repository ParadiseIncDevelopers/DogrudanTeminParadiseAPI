namespace DogrudanTeminParadiseAPI.Dto
{
    public class UserNotificationQueryDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid>? ToUsers { get; set; }
        public string? Header { get; set; }
        public int Top { get; set; } = 100;
    }
}
