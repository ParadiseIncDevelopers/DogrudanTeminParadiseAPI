using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class InspectionAcceptanceJuryDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public Guid InspectionAcceptanceJuryId { get; set; }
        public JuryType Type { get; set; }
        public List<Guid> UserIds { get; set; }
    }
}
