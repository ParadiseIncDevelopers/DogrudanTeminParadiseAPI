namespace DogrudanTeminParadiseAPI.Dto
{
    public class InspectionAcceptanceNoteDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public string Note { get; set; }
    }
}
