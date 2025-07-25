namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOSApproximateCostJuryDto
    {
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<Guid> UserIds { get; set; } = new();
    }
}
