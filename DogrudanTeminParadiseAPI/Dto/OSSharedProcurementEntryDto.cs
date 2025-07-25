namespace DogrudanTeminParadiseAPI.Dto
{
    public class OSSharedProcurementEntryDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementSharerUserId { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<Guid> SharedToUserIds { get; set; } = new();
        public DateTime SharingDate { get; set; }
    }
}
