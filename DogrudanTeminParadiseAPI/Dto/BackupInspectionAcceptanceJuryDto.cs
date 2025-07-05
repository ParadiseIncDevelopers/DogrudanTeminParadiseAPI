using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class BackupInspectionAcceptanceJuryDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementEntryId { get; set; }
        public Guid InspectionAcceptanceJuryId { get; set; }
        public JuryType Type { get; set; }
        public List<Guid> UserIds { get; set; }
        public Guid RemovedByUserId { get; set; }
        public DateTime RemovingDate { get; set; }
    }
}
