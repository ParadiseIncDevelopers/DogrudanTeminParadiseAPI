namespace DogrudanTeminParadiseAPI.Dto
{
    public class MarketPriceResearchReportDto
    {
        public string ProcurementEntryName { get; set; }
        public DateTime? ProcurementDecisionDate { get; set; }
        public string WorkReason { get; set; }
        public string ProcurementDecisionNumber { get; set; }
        public DateTime? PiyasaArastirmaBaslangicDate { get; set; }  // eklendi
        public string PiyasaArastirmaBaslangicNumber { get; set; }  // eklendi
        public WinnerDto WinnerEntreprise { get; set; }
    }

    public class WinnerDto
    {
        public string Vkn { get; set; }
        public string Unvan { get; set; }
        public double TotalOfferedPrice { get; set; }
    }
}
