namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOSProcurementEntryEditorDto
    {
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
    }
}
