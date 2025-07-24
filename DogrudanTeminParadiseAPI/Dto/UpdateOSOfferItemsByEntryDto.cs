namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateOSOfferItemsByEntryDto
    {
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<OfferItemPriceUpdateDto> Items { get; set; } = new();
    }
}
