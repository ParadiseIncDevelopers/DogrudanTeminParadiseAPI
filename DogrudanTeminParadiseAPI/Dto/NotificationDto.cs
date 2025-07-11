namespace DogrudanTeminParadiseAPI.Dto
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Header { get; set; }
        public string? Subtitle { get; set; }
        public string? Icon { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsRead { get; set; }
    }
}
