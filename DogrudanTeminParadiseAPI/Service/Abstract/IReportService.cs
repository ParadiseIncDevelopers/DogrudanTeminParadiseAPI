using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportService
    {
        Task<ApproximateCostScheduleDto> GetApproximateCostScheduleAsync(Guid procurementEntryId);
        Task<MarketPriceResearchReportDto> GetMarketPriceResearchReportAsync(Guid procurementEntryId);
        Task<InspectionAcceptanceReportDto> GetInspectionAcceptanceReportAsync(Guid procurementEntryId, DateTime invoiceDate, string invoiceNumber);
    }
}
