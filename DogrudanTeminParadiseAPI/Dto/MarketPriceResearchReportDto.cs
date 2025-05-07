namespace DogrudanTeminParadiseAPI.Dto
{
    public class MarketPriceResearchReportDto
    {
        public string ProcurementEntryName { get; set; }
        public DateTime? ProcurementDecisionDate { get; set; }
        public WinnerDto WinnerEntreprise { get; set; }
    }

    public class WinnerDto
    {
        public string Vkn { get; set; }
        public string Unvan { get; set; }
    }
}
