using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISuperAdminService
    {
        Task<string> AuthenticateAsync(SuperAdminLoginDto dto);
        Task SetUserActiveStatusAsync(ChangeUserActiveStatusDto dto);
        Task AssignUsersToAdminAsync(AssignUsersToAdminDto dto);
        Task<SystemActivityDto> GetSystemActivityAsync();
    }
}
