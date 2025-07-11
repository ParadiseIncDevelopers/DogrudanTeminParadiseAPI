﻿using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IProcurementEntryService
    {
        Task<ProcurementEntryDto> CreateAsync(CreateProcurementEntryDto dto);
        Task<IEnumerable<ProcurementEntryDto>> GetAllAsync();
        Task<IEnumerable<ProcurementEntryDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<ProcurementEntryDto> GetByIdAsync(Guid id);
        Task<ProcurementEntryDto> UpdateAsync(Guid id, UpdateProcurementEntryDto dto);
        Task DeleteAsync(Guid id, Guid userId);
        Task<IEnumerable<ProcurementEntryDto>> GetInspectionPriceRangeAsync(ProcurementEntryInspectionPriceDto query);
        Task<IEnumerable<ProcurementEntryDto>> GetByOfferCountAsync(ProcurementEntryWithOfferCountDto query);
        Task<IEnumerable<ProcurementEntryDto>> GetByAdministrativeUnitsAsync(ProcurementEntryWithUnitFilterDto query);
        Task<IEnumerable<ProcurementEntryDto>> GetByVknAsync(string vkn);
        Task<IEnumerable<ProcurementEntryDto>> GetByInspectionAcceptanceAsync();
        Task<IEnumerable<ProcurementEntryDto>> GetByBudgetAllocationAsync(Guid budgetAllocationId);
        Task<IEnumerable<ProcurementEntryDto>> GetByInspectionDateRangeAsync(ProcurementEntryDateRangeDto query);
        Task<IEnumerable<ProcurementEntryDto>> GetByRequesterAsync(Guid requesterId, bool isAdmin);

        Task<IEnumerable<ProcurementEntryDto>> GetAllUserSharedsAsync(Guid userId);

    }
}
