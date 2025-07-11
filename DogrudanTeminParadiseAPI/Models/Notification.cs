using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public string? Header { get; set; }
        public string? Subtitle { get; set; }
        public string? Icon { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsRead { get; set; }
    }
}
