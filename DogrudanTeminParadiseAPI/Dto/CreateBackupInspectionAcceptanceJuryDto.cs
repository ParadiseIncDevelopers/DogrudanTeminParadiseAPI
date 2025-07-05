using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateBackupInspectionAcceptanceJuryDto
    {
        public Guid ProcurementEntryId { get; set; }
        public Guid InspectionAcceptanceJuryId { get; set; }
        public JuryType Type { get; set; } = JuryType.INSPECTION_ACCEPTANCE;
        public List<Guid> UserIds { get; set; } = new();
        public Guid RemovedByUserId { get; set; }
        public DateTime RemovingDate { get; set; }
    }
}
