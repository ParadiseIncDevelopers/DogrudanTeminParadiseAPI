using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSProcurementEntryEditorService
    {
        Task<OSProcurementEntryEditorDto> CreateAsync(CreateOSProcurementEntryEditorDto dto);
        Task<OSProcurementEntryEditorDto> GetAsync();
        Task<OSProcurementEntryEditorDto> GetByIdAsync(Guid id);
        Task<OSProcurementEntryEditorDto> UpdateAsync(UpdateOSProcurementEntryEditorDto dto);
        Task<OSProcurementEntryEditorDto> GetEditorByEntryIdAsync(Guid entryId);
    }
}
