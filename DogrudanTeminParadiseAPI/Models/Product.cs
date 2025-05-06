using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductItemId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public string Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CategoryId { get; set; }

        public string Code { get; set; }
    }
}
