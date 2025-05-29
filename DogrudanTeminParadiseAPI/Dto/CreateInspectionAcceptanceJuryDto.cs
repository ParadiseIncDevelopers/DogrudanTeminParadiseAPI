namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateInspectionAcceptanceJuryDto
    {
        public Guid ProcurementEntryId { get; set; }
        public Guid InspectionAcceptanceJuryId { get; set; }
        public List<Guid> UserIds { get; set; } = new();
    }
}
