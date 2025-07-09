namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateUserNotificationDto
    {
        public Guid NotificationFromUser { get; set; }
        public Guid NotificationToUser { get; set; }
        public string NotificationHeader { get; set; }
        public string NotificationIcon { get; set; }
        public string NotificationText { get; set; }
        public bool IsMarkAsUnread { get; set; } = true;
    }
}
