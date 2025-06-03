using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISuperAdminService
    {
        Task<string> AuthenticateAsync(LoginDto dto);
        Task<SuperAdminDto> CreateAsync(CreateSuperAdminDto dto);
        Task<SuperAdminDto> GetByIdAsync(Guid id);
        Task<IEnumerable<SuperAdminDto>> GetAllAsync();

        Task ChangePasswordAsync(Guid userId, UpdateAdminPasswordDto dto);
        Task UpdateAsync(Guid userId, UpdateSuperAdminDto dto);
    }
}
