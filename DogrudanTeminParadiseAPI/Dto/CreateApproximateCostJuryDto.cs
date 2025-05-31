namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateApproximateCostJuryDto
    {
        public Guid ProcurementEntryId { get; set; }
        public List<Guid> UserIds { get; set; } = new();
    }
}
