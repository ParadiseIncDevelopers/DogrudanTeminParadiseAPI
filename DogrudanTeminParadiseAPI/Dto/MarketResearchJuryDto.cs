using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class MarketResearchJuryDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public JuryType Type { get; set; }
        public List<Guid> UserIds { get; set; } = [];
    }
}
