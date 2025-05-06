using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class ThreeSubAdministrationUnit
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        // Üst birim
        [BsonRepresentation(BsonType.String)]
        public Guid AdministrationUnitId { get; set; }

        // Alt birim
        [BsonRepresentation(BsonType.String)]
        public Guid SubAdministrationUnitId { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid CreatedByAdminId { get; set; }
    }
}
