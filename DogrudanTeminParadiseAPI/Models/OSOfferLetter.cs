using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OSOfferLetter
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid EntrepriseId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OneSourceProcurementEntryId { get; set; }

        public List<OfferItem> OfferItems { get; set; } = new();
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
    }
}
