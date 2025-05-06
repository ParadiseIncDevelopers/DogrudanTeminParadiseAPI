namespace DogrudanTeminParadiseAPI.Dto
{
    public class ItemCostDto
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string UnitName { get; set; }
        public List<BidDto> Bids { get; set; }
        public double AverageUnitPrice { get; set; }
        public double AverageTotalCost { get; set; }
    }
}
