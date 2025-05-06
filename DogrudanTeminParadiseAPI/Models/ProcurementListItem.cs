using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class ProcurementListItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        // İlgili ProcurementEntry
        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementEntryId { get; set; }

        public string Name { get; set; }
        public int Quantity { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UnitId { get; set; }
    }
}
