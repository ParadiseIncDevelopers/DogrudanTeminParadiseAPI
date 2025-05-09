using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOfferLetterDto
    {
        public Guid EntrepriseId { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string Title { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
    }
}
