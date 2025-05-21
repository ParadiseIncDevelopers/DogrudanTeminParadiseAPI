using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IProcurementEntryEditorService
    {
        Task<ProcurementEntryEditorDto> CreateAsync(CreateProcurementEntryEditorDto dto);
        Task<ProcurementEntryEditorDto> GetAsync();
        Task<ProcurementEntryEditorDto> GetByIdAsync(Guid id);
        Task<ProcurementEntryEditorDto> UpdateAsync(UpdateProcurementEntryEditorDto dto);
        Task<ProcurementEntryEditorDto> GetEditorByEntryIdAsync(Guid entryId);
    }
}
