namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateOSProcurementEntryEditorDto
    {
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
    }
}
