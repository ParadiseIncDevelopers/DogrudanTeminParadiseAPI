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

        public DateTime ProcurementDecisionDate { get; set; }
        public string ProcurementDecisionNumber { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid TenderResponsibleUserId { get; set; }
        public string TenderResponsibleTitle { get; set; }

        public string WorkName { get; set; }
        public string WorkReason { get; set; }
        public double BudgetAllocation { get; set; }
        public bool SpecificationToBePrepared { get; set; }
        public bool ContractToBePrepared { get; set; }

        public List<BelgeTarihVeSayilari> DocumentDatesAndNumbers { get; set; } = new();

        public Guid AdministrationUnit { get; set; }

        public Guid SubAdministrationUnit { get; set; }

        public Guid ThreeSubAdministrationUnit { get; set; }
    }
}
