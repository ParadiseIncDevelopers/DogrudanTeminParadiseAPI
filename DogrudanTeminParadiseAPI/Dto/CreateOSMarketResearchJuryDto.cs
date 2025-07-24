namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOSMarketResearchJuryDto
    {
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<Guid> UserIds { get; set; } = new();
    }
}
