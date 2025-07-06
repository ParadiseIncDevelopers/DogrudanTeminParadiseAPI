using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Filter;
using System.Text.Json.Serialization;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class BackupOfferLetterDto
    {
        public Guid Id { get; set; }
        public Guid EntrepriseId { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public List<OfferItem> OfferItems { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public Guid RemovedByUserId { get; set; }

        [JsonConverter(typeof(TurkeyDateTimeConverter))]
        public DateTime RemovingDate { get; set; }
    }
}

