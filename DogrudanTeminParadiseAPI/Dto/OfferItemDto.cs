using DogrudanTeminParadiseAPI.Helpers;

namespace DogrudanTeminParadiseAPI.Dto
{
    public class OfferItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Feature> Features { get; set; }
        public int Quantity { get; set; }
        public Guid UnitId { get; set; }
        public double UnitPrice { get; set; }
        public double TotalAmount { get; set; }
    }
}
