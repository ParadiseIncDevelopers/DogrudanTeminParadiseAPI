using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OSSharedProcurementEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementSharerUserId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OneSourceProcurementEntryId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public List<Guid> SharedToUserIds { get; set; } = new();

        public DateTime SharingDate { get; set; }
    }
}
