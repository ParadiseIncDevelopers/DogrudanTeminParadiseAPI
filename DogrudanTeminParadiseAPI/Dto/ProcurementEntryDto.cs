using DogrudanTeminParadiseAPI.Helpers;
namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementEntryDto
    {
        public Guid Id { get; set; }
        public DateTime ProcurementDecisionDate { get; set; }
        public string ProcurementDecisionNumber { get; set; }
        public Guid TenderResponsibleUserId { get; set; }
        public string TenderResponsibleTitle { get; set; }
        public string WorkName { get; set; }
        public string WorkReason { get; set; }
        public double BudgetAllocation { get; set; }
        public bool SpecificationToBePrepared { get; set; }
        public bool ContractToBePrepared { get; set; }
        public List<BelgeTarihVeSayilari> DocumentDatesAndNumbers { get; set; }
        public Guid AdministrationUnit { get; set; }
        public Guid SubAdministrationUnit { get; set; }
        public Guid ThreeSubAdministrationUnit { get; set; }
    }
}
