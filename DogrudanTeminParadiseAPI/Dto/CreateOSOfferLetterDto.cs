namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOSOfferLetterDto
    {
        public Guid EntrepriseId { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
        public string NotificationAddress { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Vkn { get; set; }
    }
}
