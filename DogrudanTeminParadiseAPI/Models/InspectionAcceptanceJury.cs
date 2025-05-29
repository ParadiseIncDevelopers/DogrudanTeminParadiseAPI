using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class InspectionAcceptanceJury
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementEntryId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid InspectionAcceptanceJuryId { get; set; }

        public JuryType Type { get; set; } = JuryType.INSPECTION_ACCEPTANCE;

        [BsonRepresentation(BsonType.String)]
        public List<Guid> UserIds { get; set; } = new();
    }
}
