namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateAdditionalInspectionAcceptanceCertificateDto
    {
        public List<OfferItemDto> SelectedProducts { get; set; } = new();
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
    }
}
