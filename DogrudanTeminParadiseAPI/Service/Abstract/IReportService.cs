using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportService
    {
        Task<ApproximateCostScheduleDto> GetApproximateCostScheduleAsync(Guid procurementEntryId);
        Task<MarketPriceResearchReportDto> GetMarketPriceResearchReportAsync(Guid procurementEntryId);
        Task<InspectionAcceptanceReportDto> GetInspectionAcceptanceReportAsync(Guid procurementEntryId, DateTime invoiceDate, string invoiceNumber);
        Task<List<BudgetItemStatsDto>> GetBudgetItemCountsAsync(int days);
        Task<InspectionPriceStatsDto> GetInspectionPriceSumAsync(int days);
        Task<List<ProductPriceStatDto>> GetTopInspectionProductsAsync(int days, int top);
        Task<List<FirmStatDto>> GetTopInspectionFirmsMonthlyAsync(int top);
    }
}
