using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Dto.Logger;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISuperAdminService
    {
        Task<string> AuthenticateAsync(SuperAdminLoginDto dto);
        Task<Dictionary<Guid, bool>> GetActivePassiveUsersAsync();
        Task SetUserActiveStatusAsync(ChangeUserActiveStatusDto dto);
        Task AssignUsersToAdminAsync(AssignUsersToAdminDto dto);
        Task<Dictionary<Guid, List<Guid>>> GetAllAdminPermissionsAsync();
        Task<List<Guid>> GetAdminPermissionsAsync(Guid adminId);
        Task ResetPasswordAsync(UpdateForgotPasswordDto dto);
        Task<IEnumerable<PageEntryDto>> GetPageActivitiesAsync(PageQueryParameters parameters);

    }
}
