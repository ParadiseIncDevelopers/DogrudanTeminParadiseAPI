namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public string? Header { get; set; }
        public string? Subtitle { get; set; }
        public string? Icon { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
