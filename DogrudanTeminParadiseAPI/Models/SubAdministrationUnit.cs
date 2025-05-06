using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class SubAdministrationUnit
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid AdministrationUnitId { get; set; }   // Üst birim
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid CreatedByAdminId { get; set; }
    }
}
