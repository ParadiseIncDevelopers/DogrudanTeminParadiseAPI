namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOSSharedProcurementEntryDto
    {
        public Guid ProcurementSharerUserId { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<Guid> SharedToUserIds { get; set; } = new();
    }
}
