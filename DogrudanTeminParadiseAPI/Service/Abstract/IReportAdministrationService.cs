using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportAdministrationService
    {
        Task<IEnumerable<TopUnitDto>> GetMostEntrySubAdministrationUnitsAsync(IEnumerable<Guid> userIds, int top = 3);
        Task<IEnumerable<TopUnitDto>> GetLeastEntrySubAdministrationUnitsAsync(IEnumerable<Guid> userIds, int top = 3);
        Task<IEnumerable<UnitPriceStatDto>> GetSubAdministrationAveragePricesAsync(string periodType);
        Task<IEnumerable<UnitPriceStatDto>> GetThreeSubAdministrationAveragePricesAsync(string periodType);
        Task<IEnumerable<TopUnitDto>> GetSubAdministrationCertificateCountsAsync();
        Task<IEnumerable<TopUnitDto>> GetThreeSubAdministrationCertificateCountsAsync();
        Task<IEnumerable<TopUnitDto>> GetSubAdministrationOfferCountsAsync();
        Task<IEnumerable<TopUnitDto>> GetThreeSubAdministrationOfferCountsAsync();
        Task<IEnumerable<UnitPriceStatDto>> GetSubAdministrationOfferTotalsAsync();
        Task<IEnumerable<UnitPriceStatDto>> GetThreeSubAdministrationOfferTotalsAsync();
    }
}
