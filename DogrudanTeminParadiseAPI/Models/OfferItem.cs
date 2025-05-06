using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OfferItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public List<Feature> Features { get; set; } = new();
        public int Quantity { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UnitId { get; set; }

        public double UnitPrice { get; set; }

        public double TotalAmount => UnitPrice * Quantity;
    }
}
