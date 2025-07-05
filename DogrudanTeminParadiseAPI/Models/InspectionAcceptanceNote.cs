using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class InspectionAcceptanceNote
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string ProcurementEntryId { get; set; }
        public string Note { get; set; }
    }
}
