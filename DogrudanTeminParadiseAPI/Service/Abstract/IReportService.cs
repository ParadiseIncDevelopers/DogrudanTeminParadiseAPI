using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportService
    {
        Task<ApproximateCostScheduleDto> GetApproximateCostScheduleAsync(Guid procurementEntryId);
        Task<MarketPriceResearchReportDto> GetMarketPriceResearchReportAsync(Guid procurementEntryId);
        Task<InspectionAcceptanceReportDto> GetInspectionAcceptanceReportAsync(Guid procurementEntryId, DateTime invoiceDate, string invoiceNumber);
        Task<List<TopUnitDto>> GetTopBudgetAllocationsAsync(Guid tenderResponsibleUserId, int top);
        Task<InspectionPriceStatsDto> GetInspectionPriceSumAsync(int days);
        Task<List<ProductPriceStatDto>> GetTopInspectionProductsAsync(int days, int top);
        Task<List<FirmStatDto>> GetTopInspectionFirmsMonthlyAsync(int top);
        Task<List<LastJobsDto>> GetLast10JobsAsync(Guid tenderResponsibleId);
        Task<List<TopUnitDto>> GetTopAdministrationUnitsAsync(Guid tenderResponsibleId);
        Task<List<TopUnitDto>> GetTopSubAdministrationUnitsAsync(Guid tenderResponsibleId);
        Task<List<TopUnitDto>> GetTopThreeSubAdministrationUnitsAsync(Guid tenderResponsibleId);
        Task<SpendingReportDto> GetSpendingReportAsync(Guid tenderResponsibleId);
        Task<SpendingByFirmDto> GetTopFirmsSpendingAsync();
        Task<IEnumerable<UserCountDto>> GetTopResponsibleUsersAsync(int top = 3);
        Task<IEnumerable<UserCountDto>> GetBottomResponsibleUsersAsync(int bottom = 3);
    }
}
