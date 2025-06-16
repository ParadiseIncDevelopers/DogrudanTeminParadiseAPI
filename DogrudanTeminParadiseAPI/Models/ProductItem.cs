using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Models
{
    public class ProductItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CategoryId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ProductItemType Type { get; set; }
    }
}
