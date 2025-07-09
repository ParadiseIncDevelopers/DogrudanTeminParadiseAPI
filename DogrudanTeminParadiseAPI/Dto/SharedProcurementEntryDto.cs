namespace DogrudanTeminParadiseAPI.Dto
{
    public class SharedProcurementEntryDto
    {
        public Guid Id { get; set; }
        public Guid ProcurementSharerUserId { get; set; }
        public Guid ProcurementId { get; set; }
        public List<Guid> SharedToUserIds { get; set; }
        public DateTime SharingDate { get; set; }
    }
}
