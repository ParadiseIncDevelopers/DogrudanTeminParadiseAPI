using DogrudanTeminParadiseAPI.Filter;
using System.Text.Json.Serialization;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class BackupProcurementEntryEditorDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public List<OfferItemDto> OfferItems { get; set; } = new();
        public Guid RemovedByUserId { get; set; }

        [JsonConverter(typeof(TurkeyDateTimeConverter))]
        public DateTime RemovingDate { get; set; }
    }
}
