using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OSInspectionAcceptanceJury
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OneSourceProcurementEntryId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid InspectionAcceptanceJuryId { get; set; }

        public JuryType Type { get; set; } = JuryType.INSPECTION_ACCEPTANCE;

        [BsonRepresentation(BsonType.String)]
        public List<Guid> UserIds { get; set; } = new();
    }
}
