using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class DecisionNumbers
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public ProcurementNumbers ProcurementNumbers { get; set; } = new();

        public string Name { get; set; }
    }
}
