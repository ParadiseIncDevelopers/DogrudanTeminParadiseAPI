
namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ProductItemId { get; set; }
    }
}
