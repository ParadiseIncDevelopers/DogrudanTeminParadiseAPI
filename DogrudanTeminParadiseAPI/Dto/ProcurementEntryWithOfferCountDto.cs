using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementEntryWithOfferCountDto
    {
        [BsonIgnore]
        public int? MinCount { get; set; }

        [BsonIgnore]
        public int? MaxCount { get; set; }

        public Guid ProcurementEntryId { get; set; }

        public string WorkName { get; set; }

        public int OfferCount { get; set; }
    }
}
