using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Models
{
    public class OSInspectionAcceptanceCertificate
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid OneSourceProcurementEntryId { get; set; }

        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }

        public List<SelectedOfferItem> SelectedProducts { get; set; } = new();

        [BsonRepresentation(BsonType.String)]
        public Guid AdministrationUnitId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid SubAdministrationUnitId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid ThreeSubAdministrationUnitId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Guid SelectedOfferLetterId { get; set; }
    }
}
