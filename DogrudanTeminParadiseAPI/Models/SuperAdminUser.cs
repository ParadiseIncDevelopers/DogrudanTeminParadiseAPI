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

        public Dictionary<Guid, bool> ActivePassiveUsers { get; set; } = [];
        public Dictionary<Guid, List<Guid>> AssignPermissionToAdmin { get; set; } = [];
    }
}
