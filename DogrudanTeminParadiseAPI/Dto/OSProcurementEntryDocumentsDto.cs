namespace DogrudanTeminParadiseAPI.Dto
{
    public class OSProcurementEntryDocumentsDto
    {
        public Guid Id { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<string> EntrepriseFileIds { get; set; }
        public DateTime TransactionAt { get; set; }
    }
}
