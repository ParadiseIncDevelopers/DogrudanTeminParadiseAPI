using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class AdminUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Tcid { get; set; }

        /// <summary>
        /// "Admin" veya "User"
        /// </summary>
        public string UserType { get; set; }

        public List<string> Permissions { get; set; } = [];

        /// <summary>
        /// Sadece kamu kurumu atanan admin’ler için
        /// </summary>
        [BsonElement("PublicInstitutionName")]
        public string PublicInstitutionName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? TitleId { get; set; } = null;
    }
}
