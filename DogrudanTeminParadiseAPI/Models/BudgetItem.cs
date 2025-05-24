using DogrudanTeminParadiseAPI.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Models
{
    public class BudgetItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public string FinancialCode { get; set; }
        public string EconomyCode { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid CreatedByAdminId { get; set; }

        public List<BudgetRecord> Records { get; set; } = new();
    }
}
