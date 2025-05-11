namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateInspectionAcceptanceCertificateDto
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<OfferItemDto> SelectedProducts { get; set; } = new();
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
    }
}
