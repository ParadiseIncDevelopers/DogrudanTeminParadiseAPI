namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateAdditionalInspectionAcceptanceCertificateDto
    {
        public Guid ProcurementEntryId { get; set; }
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
        public List<OfferItemDto> SelectedProducts { get; set; } = new();
        public Guid MainInspectionAcceptanceId { get; set; }
    }
}
