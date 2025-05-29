using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IUserService
    {
        Task<string> AuthenticateAsync(LoginDto dto);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> GetProfileAsync(Guid userId);
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
        Task DeleteAsync(Guid id);
        Task<UserDto> AssignTitleAsync(Guid userId, Guid titleId);

    }
}
