namespace DogrudanTeminParadiseAPI.Dto
{
    public class OSInspectionAcceptanceNoteDto
    {
        public Guid Id { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public string Note { get; set; }
    }
}
