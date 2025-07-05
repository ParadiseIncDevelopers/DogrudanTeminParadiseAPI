using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Filter;
using System.Text.Json.Serialization;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateBackupAdditionalInspectionAcceptanceCertificateDto
    {
        public Guid ProcurementEntryId { get; set; }
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
        public List<SelectedOfferItem> SelectedProducts { get; set; } = new();
        public Guid SelectedOfferLetterId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public Guid RemovedByUserId { get; set; }
        [JsonConverter(typeof(TurkeyDateTimeConverter))]
        public DateTime RemovingDate { get; set; }
    }
}
