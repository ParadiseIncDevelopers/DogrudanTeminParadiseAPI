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

    public class Feature
    {
        public string Description { get; set; }
    }

    public class BudgetRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public double Price { get; set; }
        public string InvoiceNumber { get; set; }
        public string WorkName { get; set; }
        public string WorkReason { get; set; }
    }
}
