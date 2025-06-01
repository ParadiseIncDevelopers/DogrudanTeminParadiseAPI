using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IProcurementEntryService
    {
        Task<ProcurementEntryDto> CreateAsync(CreateProcurementEntryDto dto);
        Task<IEnumerable<ProcurementEntryDto>> GetAllAsync();
        Task<ProcurementEntryDto> GetByIdAsync(Guid id);
        Task<ProcurementEntryDto> UpdateAsync(Guid id, UpdateProcurementEntryDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<ProcurementEntryInspectionPriceDto>> GetInspectionPriceRangeAsync(ProcurementEntryInspectionPriceDto query);
        Task<IEnumerable<ProcurementEntryWithOfferCountDto>> GetByOfferCountAsync(ProcurementEntryWithOfferCountDto query);
        Task<IEnumerable<ProcurementEntryWithUnitFilterDto>> GetByAdministrativeUnitsAsync(ProcurementEntryWithUnitFilterDto query);
        Task<IEnumerable<ProcurementEntryDto>> GetByVknAsync(string vkn);
        Task<IEnumerable<ProcurementEntryDto>> GetByInspectionAcceptanceAsync();
        Task<IEnumerable<ProcurementEntryDto>> GetByRequesterAsync(Guid requesterId, bool isAdmin);

    }
}
