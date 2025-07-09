namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateSharedProcurementEntryDto
    {
        public Guid ProcurementSharerUserId { get; set; }
        public Guid ProcurementId { get; set; }
        public List<Guid> SharedToUserIds { get; set; } = new();
    }
}
