using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class OSMarketResearchJuryDto
    {
        public Guid Id { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public JuryType Type { get; set; }
        public List<Guid> UserIds { get; set; } = [];
    }
}
