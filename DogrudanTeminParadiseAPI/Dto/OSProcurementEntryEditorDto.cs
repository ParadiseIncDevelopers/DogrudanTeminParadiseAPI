namespace DogrudanTeminParadiseAPI.Dto
{
    public class OSProcurementEntryEditorDto
    {
        public Guid Id { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<OfferItemDto> OfferItems { get; set; }
    }
}
