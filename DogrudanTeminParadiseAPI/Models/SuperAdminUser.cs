using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class SuperAdminUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string UserType { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Dictionary<string, bool> ActivePassiveUsers { get; set; } = [];
        [BsonRepresentation(BsonType.String)]
        public Dictionary<string, List<string>> AssignPermissionToAdmin { get; set; } = [];
    }
}
