using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Models
{
    public class InspectionAcceptanceCertificate
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProcurementEntryId { get; set; }

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
