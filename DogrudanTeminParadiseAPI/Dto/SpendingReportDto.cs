namespace DogrudanTeminParadiseAPI.Dto
{
    public class SpendingReportDto
    {
        public List<TimeSeriesDataDto> Weekly { get; set; }
        public List<TimeSeriesDataDto> Monthly { get; set; }
        public List<TimeSeriesDataDto> Quarterly { get; set; }
        public List<TimeSeriesDataDto> Yearly { get; set; }
    }
}
