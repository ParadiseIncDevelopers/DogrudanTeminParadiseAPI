using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class ProcurementEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public DateTime? ProcurementDecisionDate { get; set; }
        public string? ProcurementDecisionNumber { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? TenderResponsibleUserId { get; set; }
        public string? TenderResponsibleTitle { get; set; }

        public string? WorkName { get; set; }
        public string? WorkReason { get; set; }
        public double? BudgetAllocation { get; set; }
        public bool SpecificationToBePrepared { get; set; }
        public bool ContractToBePrepared { get; set; }

        public DateTime? PiyasaArastirmaOnayDate { get; set; }
        public string? PiyasaArastirmaOnayNumber { get; set; }

        public DateTime? TeklifMektubuDate { get; set; }
        public string? TeklifMektubuNumber { get; set; }

        public DateTime? PiyasaArastirmaBaslangicDate { get; set; }
        public string? PiyasaArastirmaBaslangicNumber { get; set; }

        public DateTime? YaklasikMaliyetHesaplamaBaslangicDate { get; set; }
        public string? YaklasikMaliyetHesaplamaBaslangicNumber { get; set; }

        public DateTime? MuayeneVeKabulBelgesiDate { get; set; }
        public string? MuayeneVeKabulBelgesiNumber { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? AdministrationUnitId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? SubAdministrationUnitId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? ThreeSubAdministrationUnitId { get; set; }
    }
}
