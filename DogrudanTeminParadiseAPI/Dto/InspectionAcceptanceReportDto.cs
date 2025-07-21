namespace DogrudanTeminParadiseAPI.Dto
{
    public class InspectionAcceptanceReportDto
    {
        public string EntrepriseUnvan { get; set; }
        public string ThreeSubAdministrationUnitName { get; set; }
        public DateTime? ProcurementDecisionDate { get; set; }
        public string ProcurementDecisionNumber { get; set; }
        public string DosyaNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public List<InspectionItemDto> Items { get; set; }
    }

    public class InspectionItemDto
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string UnitName { get; set; }
    }
}
