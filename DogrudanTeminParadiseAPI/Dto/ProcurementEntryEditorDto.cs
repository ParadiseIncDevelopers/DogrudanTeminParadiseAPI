using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementEntryEditorDto
    {
        public Guid Id { get; set; }
        public string EntryHTML { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public List<OfferItemDto> OfferItems { get; set; }
    }
}
