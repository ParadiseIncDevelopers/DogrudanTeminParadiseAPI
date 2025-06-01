using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IAdminUserService
    {
        Task<string> AuthenticateAsync(LoginDto dto);
        Task<AdminUserDto> CreateAsync(CreateUserDto dto);
        Task<AdminUserDto> GetByIdAsync(Guid id);
        Task<IEnumerable<AdminUserDto>> GetAllAsync();

        Task ChangePasswordAsync(Guid userId, UpdateAdminPasswordDto dto);
        Task AssignTitleAsync(Guid userId, Guid titleId);
    }
}
