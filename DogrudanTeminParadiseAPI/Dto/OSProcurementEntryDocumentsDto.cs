namespace DogrudanTeminParadiseAPI.Dto
{
    public class OSProcurementEntryDocumentsDto
    {
        public Guid Id { get; set; }
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<byte[]> EntrepriseFiles { get; set; }
        public DateTime TransactionAt { get; set; }
    }
}
