using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class OSAdditionalInspectionAcceptanceCertificateDto
    {
        public Guid Id { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
        public List<SelectedOfferItem> SelectedProducts { get; set; }
        public Guid SelectedOfferLetterId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
