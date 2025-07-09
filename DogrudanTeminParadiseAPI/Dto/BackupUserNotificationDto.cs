using DogrudanTeminParadiseAPI.Filter;
using System.Text.Json.Serialization;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class BackupUserNotificationDto
    {
        public Guid Id { get; set; }
        public Guid NotificationFromUser { get; set; }
        public Guid NotificationToUser { get; set; }
        public string NotificationHeader { get; set; }
        public string NotificationIcon { get; set; }
        public string NotificationText { get; set; }
        public DateTime NotificationDate { get; set; }
        public Guid RemovedByUserId { get; set; }

        [JsonConverter(typeof(TurkeyDateTimeConverter))]
        public DateTime RemovingDate { get; set; }
    }
}
