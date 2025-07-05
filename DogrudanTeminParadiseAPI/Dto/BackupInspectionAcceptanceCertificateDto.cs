using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Filter;
using System.Text.Json.Serialization;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class BackupInspectionAcceptanceCertificateDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<SelectedOfferItem> SelectedProducts { get; set; }
        public Guid SelectedOfferLetterId { get; set; }
        public Guid AdministrationUnitId { get; set; }
        public Guid SubAdministrationUnitId { get; set; }
        public Guid ThreeSubAdministrationUnitId { get; set; }
        public Guid RemovedByUserId { get; set; }
        [JsonConverter(typeof(TurkeyDateTimeConverter))]
        public DateTime RemovingDate { get; set; }
    }
}
