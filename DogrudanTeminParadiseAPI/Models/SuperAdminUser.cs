using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class SuperAdminUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        /// <summary>
        /// SHA512 ile hashlenmiş parola
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// AES ile şifrelenmiş TC kimlik numarası
        /// </summary>
        public string Tcid { get; set; }

        /// <summary>
        /// Bu varlık için sabit olarak "SUPER_ADMIN" atanacak
        /// </summary>
        public string UserType { get; set; } = "SUPER_ADMIN";

        // SuperAdmin için Permissions alanı yoktur.
        // SuperAdmin için TitleId veya PublicInstitutionName alanı yoktur.
    }
}
