using MongoDB.Bson.Serialization.Attributes;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProcurementEntryWithUnitFilterDto
    {
        [BsonIgnore]
        public Guid? AdministrationUnitId { get; set; }

        [BsonIgnore]
        public Guid? SubAdministrationUnitId { get; set; }

        [BsonIgnore]
        public Guid? ThreeSubAdministrationUnitId { get; set; }

        public Guid ProcurementEntryId { get; set; }

        public string WorkName { get; set; }
    }
}
