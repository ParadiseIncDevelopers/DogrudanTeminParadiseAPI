using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IAdminUserService
    {
        Task<string> AuthenticateAsync(LoginDto dto);
        Task<AdminUserDto> CreateAsync(CreateUserDto dto);
        Task<AdminUserDto> GetByIdAsync(Guid id);
        /// <summary>
        /// Tüm admin user'ları listeler.
        /// </summary>
        Task<IEnumerable<AdminUserDto>> GetAllAsync();
    }
}
