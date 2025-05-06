namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateBudgetItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public Guid CreatedByAdminId { get; set; }
    }
}
