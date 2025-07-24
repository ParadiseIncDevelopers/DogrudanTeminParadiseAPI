using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OSProcurementEntryDocuments
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OneSourceProcurementEntryId { get; set; }

        public List<byte[]> EntrepriseFiles { get; set; } = new();

        public DateTime TransactionAt { get; set; }
    }
}
