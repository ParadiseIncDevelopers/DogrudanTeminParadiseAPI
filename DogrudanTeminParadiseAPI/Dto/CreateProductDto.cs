namespace DogrudanTeminParadiseAPI.Dto
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ProductItemId { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
    }
}
