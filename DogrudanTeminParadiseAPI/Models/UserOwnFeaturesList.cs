using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class UserOwnFeaturesList
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<UserFeatures> FeaturesLists { get; set; } = [];
    }
}
