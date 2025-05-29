namespace DogrudanTeminParadiseAPI.Dto
{
    public class TimeSeriesPointDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class BudgetItemStatsDto
    {
        public Guid BudgetAllocationId { get; set; }
        public List<TimeSeriesPointDto> DataPoints { get; set; }
    }

    public class InspectionPriceStatsDto
    {
        /// <summary>Period in days: e.g. 7, 30, 90, 365</summary>
        public int Days { get; set; }
        /// <summary>Total combined price of normal and additional inspections</summary>
        public double TotalPrice { get; set; }
    }

    public class ProductPriceStatDto
    {
        public string Name { get; set; }
        public double TotalPrice { get; set; }
    }

    public class FirmStatDto
    {
        public string Vkn { get; set; }
        public string Unvan { get; set; }
        public int Count { get; set; }
    }
}
