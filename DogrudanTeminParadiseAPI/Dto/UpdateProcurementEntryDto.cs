namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateProcurementEntryDto
    {
        public DateTime? ProcurementDecisionDate { get; set; }
        public string? ProcurementDecisionNumber { get; set; }
        public Guid TenderResponsibleUserId { get; set; }
        public string? TenderResponsibleTitle { get; set; }
        public string? WorkName { get; set; }
        public string? WorkReason { get; set; }
        public Guid? BudgetAllocationId { get; set; }
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
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
    }
}
