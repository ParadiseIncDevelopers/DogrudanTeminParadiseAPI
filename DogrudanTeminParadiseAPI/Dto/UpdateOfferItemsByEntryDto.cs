namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateOfferItemsByEntryDto
    {
        public Guid ProcurementEntryId { get; set; }
        public List<OfferItemPriceUpdateDto> Items { get; set; } = new();
    }
}
