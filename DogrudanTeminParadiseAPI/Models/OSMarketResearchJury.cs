using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OSMarketResearchJury
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OneSourceProcurementEntryId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public JuryType Type { get; set; } = JuryType.MARKET_RESEARCH;

        [BsonRepresentation(BsonType.String)]
        public List<Guid> UserIds { get; set; } = new();
    }
}
