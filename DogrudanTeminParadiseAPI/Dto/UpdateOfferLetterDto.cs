using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateOfferLetterDto
    {
        public List<UpdateOfferItemDto> OfferItems { get; set; } = new();
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
    }
}
