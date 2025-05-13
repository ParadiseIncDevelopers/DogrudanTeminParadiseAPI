using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateInspectionAcceptanceCertificateDto
    {
        public Guid ProcurementEntryId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<SelectedOfferItem> SelectedProducts { get; set; } = new();
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
        public Guid SelectedOfferLetterId { get; set; }
    }
}
