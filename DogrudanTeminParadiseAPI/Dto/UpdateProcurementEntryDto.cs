using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateProcurementEntryDto
    {
        public DateTime ProcurementDecisionDate { get; set; }
        public string ProcurementDecisionNumber { get; set; }
        public Guid TenderResponsibleUserId { get; set; }
        public string TenderResponsibleTitle { get; set; }
        public string WorkName { get; set; }
        public string WorkReason { get; set; }
        public double BudgetAllocation { get; set; }
        public bool SpecificationToBePrepared { get; set; }
        public bool ContractToBePrepared { get; set; }
        public List<BelgeTarihVeSayilari> DocumentDatesAndNumbers { get; set; } = new();
    }
}
