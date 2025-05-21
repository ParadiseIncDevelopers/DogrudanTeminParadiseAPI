using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateOfferLetterDto
    {
        public Guid OfferItemsId { get; set; }
        public string NotificationAddress { get; set; }
        public string Nationality { get; set; }
        public string ResponsiblePerson { get; set; }
    }
}
