namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOfferLetterDto
    {
        public Guid EntrepriseId { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; }
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
    }
}
