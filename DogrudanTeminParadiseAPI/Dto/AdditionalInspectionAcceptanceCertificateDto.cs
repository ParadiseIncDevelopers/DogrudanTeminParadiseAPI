using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class AdditionalInspectionAcceptanceCertificateDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
        public List<SelectedOfferItem> SelectedProducts { get; set; }
        public Guid MainInspectionAcceptanceId { get; set; }
    }
}
