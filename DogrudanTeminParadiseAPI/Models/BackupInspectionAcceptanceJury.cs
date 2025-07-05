using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class BackupInspectionAcceptanceJury
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

        [BsonRepresentation(BsonType.String)]
        public Guid RemovedByUserId { get; set; }
        public DateTime RemovingDate { get; set; }
    }
}
