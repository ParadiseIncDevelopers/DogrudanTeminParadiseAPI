using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class MarketResearchJuryDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public JuryType Type { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
