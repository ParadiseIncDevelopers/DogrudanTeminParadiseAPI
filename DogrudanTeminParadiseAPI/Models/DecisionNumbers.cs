using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class DecisionNumbers
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public string PiyasaArastirmaOnayNumber { get; set; }

        public DateTime PiyasaArastirmaOnayDate { get; set; }

        public string Name { get; set; }
    }
}
