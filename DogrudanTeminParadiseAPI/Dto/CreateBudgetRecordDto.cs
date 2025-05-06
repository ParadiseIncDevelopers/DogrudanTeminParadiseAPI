namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateBudgetRecordDto
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public string InvoiceNumber { get; set; }
        public string WorkName { get; set; }
        public string WorkReason { get; set; }
    }
}
