using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementEntryWithOfferCountDto
    {
        [BsonIgnore]
        public int? MinOfferPrice { get; set; }

        [BsonIgnore]
        public int? MaxOfferPrice { get; set; }
    }
}
