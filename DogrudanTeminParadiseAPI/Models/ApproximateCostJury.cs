using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class ApproximateCostJury
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementEntryId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public JuryType Type { get; set; } = JuryType.APPROXIMATE_COST;

        [BsonRepresentation(BsonType.String)]
        public List<Guid> UserIds { get; set; } = new();
    }
}
