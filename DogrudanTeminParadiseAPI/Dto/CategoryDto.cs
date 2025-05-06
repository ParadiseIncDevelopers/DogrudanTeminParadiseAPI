namespace DogrudanTeminParadiseAPI.Dto
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid CreatedByAdminId { get; set; }
    }
}
