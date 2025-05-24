using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class ProductItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public ProductItemType Type { get; set; }
    }
}
