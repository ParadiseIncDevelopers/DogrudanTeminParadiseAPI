using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace DogrudanTeminParadiseAPI.Helpers
{
    public class BelgeTarihVeSayilari
    {
        public DateTime Date { get; set; }
        public string Number { get; set; }
    }

    public class UserFeatures
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        public string ListName { get; set; }

        public List<Feature> Features { get; set; } = new();
    }

    public class Feature
    {
        public string Description { get; set; }
    }

    public class BudgetRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Date { get; set; }
        public double Price { get; set; }
        public string InvoiceNumber { get; set; }
        public string WorkName { get; set; }
        public string WorkReason { get; set; }
    }

    public enum JuryType
    {
        MARKET_RESEARCH,
        INSPECTION_ACCEPTANCE,
        SUB_INSPECTION_ACCEPTANCE,
        APPROXIMATE_COST
    }

    public class SelectedOfferItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public List<Feature> Features { get; set; } = new();
        public double Quantity { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UnitId { get; set; }

        public double UnitPrice { get; set; }
    }

    public enum ProductItemType
    {
        PRODUCT,
        SERVICE
    }

    public enum KanunMaddesi
    {
        _22A,
        _22B,
        _22C
    }
}
