using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportService
    {
        Task<ApproximateCostScheduleDto> GetApproximateCostScheduleAsync(Guid procurementEntryId);
        Task<MarketPriceResearchReportDto> GetMarketPriceResearchReportAsync(Guid procurementEntryId);
        Task<InspectionAcceptanceReportDto> GetInspectionAcceptanceReportAsync(Guid procurementEntryId, DateTime invoiceDate, string invoiceNumber);
        Task<List<TopUnitDto>> GetTopBudgetAllocationsAsync(IEnumerable<Guid> tenderResponsibleUserIds, int top);
        Task<InspectionPriceStatsDto> GetInspectionPriceSumAsync(int days);
        Task<List<ProductPriceStatDto>> GetTopInspectionProductsAsync(int days, int top);
        Task<List<FirmStatDto>> GetTopInspectionFirmsMonthlyAsync(int top);
        Task<List<LastJobsDto>> GetLast10JobsAsync(IEnumerable<Guid> tenderResponsibleIds);
        Task<List<TopUnitDto>> GetTopAdministrationUnitsAsync(IEnumerable<Guid> tenderResponsibleIds);
        Task<List<TopUnitDto>> GetTopSubAdministrationUnitsAsync(IEnumerable<Guid> tenderResponsibleIds);
        Task<List<TopUnitDto>> GetTopThreeSubAdministrationUnitsAsync(IEnumerable<Guid> tenderResponsibleIds);
        Task<SpendingReportDto> GetSpendingReportAsync(IEnumerable<Guid> tenderResponsibleIds);
        Task<SpendingByFirmDto> GetTopFirmsSpendingAsync(string periodType);
        Task<IEnumerable<UserCountDto>> GetTopResponsibleUsersAsync(int top = 3);
        Task<IEnumerable<UserCountDto>> GetBottomResponsibleUsersAsync(int bottom = 3);
        Task<IEnumerable<BudgetAllocationEntryReportDto>> OnGetBudgetAllocationsEntryReports(
            IEnumerable<Guid> userIds,
            string economyCode,
            string financialCode);
    }
}
