using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> GetProfileAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
        Task DeleteAsync(Guid id);
        Task<User> AuthenticateAsync(string tcid, string password);
        Task<UserDto> AssignTitleAsync(Guid userId, Guid titleId);

    }
}
