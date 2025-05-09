using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateOfferLetterDto
    {
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
        public string NotificationAddress { get; set; }
        public string Nationality { get; set; }
        public string ResponsiblePerson { get; set; }
    }
}
