namespace DogrudanTeminParadiseAPI.Dto
{
    public class LastJobsDto
    {
        public Guid ProcurementEntryId { get; set; }
        public string WorkName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
