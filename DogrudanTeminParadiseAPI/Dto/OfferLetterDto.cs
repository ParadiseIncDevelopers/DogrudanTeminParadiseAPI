using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class OfferLetterDto
    {
        public Guid Id { get; set; }
        public Guid EntrepriseId { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public List<OfferItemDto> OfferItems { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
    }
}
