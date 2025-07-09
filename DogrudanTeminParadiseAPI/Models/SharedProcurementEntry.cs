using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class SharedProcurementEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementSharerUserId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public List<Guid> SharedToUserIds { get; set; } = new();

        public DateTime SharingDate { get; set; }
    }
}
