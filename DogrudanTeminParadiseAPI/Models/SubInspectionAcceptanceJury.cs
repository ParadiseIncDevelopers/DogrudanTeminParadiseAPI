using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class SubInspectionAcceptanceJury
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public List<Guid> UserIds { get; set; } = new();
        public JuryType Type { get; set; } = JuryType.SUB_INSPECTION_ACCEPTANCE;
    }
}
