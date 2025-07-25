namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateOSProcurementEntryDocumentsDto
    {
        public Guid OneSourceProcurementEntryId { get; set; }
        public List<byte[]> EntrepriseFiles { get; set; } = new();
    }
}
