using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class UserNotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid NotificationFromUser { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid NotificationToUser { get; set; }

        public string NotificationHeader { get; set; }
        public string NotificationIcon { get; set; }
        public string NotificationText { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsMarkAsUnread { get; set; }
    }
}
