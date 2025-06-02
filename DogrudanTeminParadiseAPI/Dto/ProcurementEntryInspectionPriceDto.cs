using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementEntryInspectionPriceDto
    {
        [BsonIgnore]
        public double? MinOfferPrice { get; set; }

        [BsonIgnore]
        public double? MaxOfferPrice { get; set; }
    }
}
