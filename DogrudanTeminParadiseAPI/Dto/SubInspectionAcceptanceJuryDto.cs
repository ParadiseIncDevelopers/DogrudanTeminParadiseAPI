using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class SubInspectionAcceptanceJuryDto
    {
        public Guid Id { get; set; }
        public List<Guid> UserIds { get; set; }
        public JuryType Type { get; set; }
    }
}
