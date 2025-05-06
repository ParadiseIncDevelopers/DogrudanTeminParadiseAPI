using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class AssignedPersonnel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementEntryId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId1 { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId2 { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId3 { get; set; }
    }
}
