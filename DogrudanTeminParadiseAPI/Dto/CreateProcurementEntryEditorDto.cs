namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateProcurementEntryEditorDto
    {
        public string EntryHTML { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public List<CreateOfferItemDto> OfferItems { get; set; } = new();
    }
}
