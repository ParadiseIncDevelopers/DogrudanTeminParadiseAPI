namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid CreatedByAdminId { get; set; }
    }
}
