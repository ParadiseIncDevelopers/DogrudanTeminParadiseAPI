namespace DogrudanTeminParadiseAPI.Dto
{
    public class UserNotificationDto
    {
        public Guid Id { get; set; }
        public Guid NotificationFromUser { get; set; }
        public Guid NotificationToUser { get; set; }
        public string NotificationHeader { get; set; }
        public string NotificationIcon { get; set; }
        public string NotificationText { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsMarkAsUnread { get; set; }
    }
}
