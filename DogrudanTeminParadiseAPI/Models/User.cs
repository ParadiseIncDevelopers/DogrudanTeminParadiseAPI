using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Tcid { get; set; }

        public List<string> Permissions { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid? TitleId { get; set; }
        public string UserType { get; set; } = "User"; // Default olarak "User" yazıyoruz
    }
}
