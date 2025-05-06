namespace DogrudanTeminParadiseAPI.Dto
{
    public class BudgetItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public Guid CreatedByAdminId { get; set; }
        public List<BudgetRecordDto> Records { get; set; }
    }
}
