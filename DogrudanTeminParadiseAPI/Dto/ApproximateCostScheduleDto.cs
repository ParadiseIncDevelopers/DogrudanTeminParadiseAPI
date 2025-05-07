namespace DogrudanTeminParadiseAPI.Dto
{
    public class ApproximateCostScheduleDto
    {
        public string ProcurementEntryName { get; set; }
        public DateTime? ProcurementDecisionDate { get; set; }
        public List<ItemCostDto> Items { get; set; }
        public double AverageTotalCostSum { get; set; } // tespit edilen toplam yaklaşık maliyet tutarı
    }
}
