namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateProcurementEntryEditorDto
    {
        public Guid ProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
    }
}
