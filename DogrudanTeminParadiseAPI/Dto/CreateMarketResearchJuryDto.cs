namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateMarketResearchJuryDto
    {
        public Guid ProcurementEntryId { get; set; }
        public List<Guid> UserIds { get; set; } = new();
    }
}
