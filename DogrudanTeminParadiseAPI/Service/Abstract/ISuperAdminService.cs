using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISuperAdminService
    {
        Task<string> AuthenticateAsync(SuperAdminLoginDto dto);
        Task<Dictionary<Guid, bool>> GetActivePassiveUsersAsync();
        Task SetUserActiveStatusAsync(ChangeUserActiveStatusDto dto);
        Task AssignUsersToAdminAsync(AssignUsersToAdminDto dto);
        Task<SystemActivityDto> GetSystemActivityAsync();
        Task<Dictionary<Guid, List<Guid>>> GetAllAdminPermissionsAsync();
        Task<List<Guid>> GetAdminPermissionsAsync(Guid adminId);
    }
}
