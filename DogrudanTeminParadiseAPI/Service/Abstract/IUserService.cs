﻿using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(CreateUserDto dto, string adminId);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> GetProfileAsync(Guid userId);
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
        Task DeleteAsync(Guid id);
        Task ChangePasswordAsync(Guid userId, UpdateUserPasswordDto dto);
        Task AssignTitleAsync(Guid userId, Guid titleId);

    }
}
