namespace DogrudanTeminParadiseAPI.Dto
{
    public class BudgetAllocationEntryReportDto
    {
        public string EconomyCode { get; set; }
        public string FinancialCode { get; set; }
        public string BudgetItemName { get; set; }
        public List<BudgetAllocationEntryDto> ProcurementEntries { get; set; }
    }

    public class BudgetAllocationEntryDto
    {
        public Guid ProcurementEntryId { get; set; }
        public string WorkName { get; set; }
        public string WorkReason { get; set; }
        public Guid? TenderResponsibleUserId { get; set; }
        public string TenderResponsibleName { get; set; }
        public Guid? AdministrationUnitId { get; set; }
        public string AdministrationUnitName { get; set; }
        public Guid? SubAdministrationUnitId { get; set; }
        public string SubAdministrationUnitName { get; set; }
        public Guid? ThreeSubAdministrationUnitId { get; set; }
        public string ThreeSubAdministrationUnitName { get; set; }
    }
}
