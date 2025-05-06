namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ProductItemId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}
