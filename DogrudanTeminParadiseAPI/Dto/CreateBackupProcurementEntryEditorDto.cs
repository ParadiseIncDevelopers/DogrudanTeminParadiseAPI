using DogrudanTeminParadiseAPI.Filter;
using System.Text.Json.Serialization;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateBackupProcurementEntryEditorDto
    {
        public Guid ProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
        public Guid RemovedByUserId { get; set; }

        [JsonConverter(typeof(TurkeyDateTimeConverter))]
        public DateTime RemovingDate { get; set; }
    }
}
