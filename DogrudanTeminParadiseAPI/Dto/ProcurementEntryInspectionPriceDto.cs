using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementEntryInspectionPriceDto
    {
        [BsonIgnore]
        public double? MinPrice { get; set; }

        [BsonIgnore]
        public double? MaxPrice { get; set; }

        public Guid ProcurementEntryId { get; set; }

        public double PriceMin { get; set; }

        public double PriceMax { get; set; }
    }
}
