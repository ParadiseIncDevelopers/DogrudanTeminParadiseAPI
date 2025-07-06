using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Models
{
    public class BackupOfferLetter
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid EntrepriseId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementEntryId { get; set; }

        public List<OfferItem> OfferItems { get; set; } = new();
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid RemovedByUserId { get; set; }
        public DateTime RemovingDate { get; set; }
    }
}

