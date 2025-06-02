namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateProcurementEntryEditorDto
    {
        public Guid ProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
    }
}
