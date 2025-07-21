using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class UpdateOfferItemDto
    {
        public string Name { get; set; }
        public List<Feature> Features { get; set; } = new();
        public double Quantity { get; set; }
        public Guid UnitId { get; set; }
        public double UnitPrice { get; set; }
    }
}
