using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OfferLetter
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid EntrepriseId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementEntryId { get; set; }

        // Teklif Kalemleri
        public List<OfferItem> OfferItems { get; set; } = new();

        // Firma bilgileri otomatik çekilecek
        public string Title { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
    }
}
