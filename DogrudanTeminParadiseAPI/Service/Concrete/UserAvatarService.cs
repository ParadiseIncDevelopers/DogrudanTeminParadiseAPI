using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using MongoDB.Driver;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class UserAvatarService : IUserAvatarService
    {
        private readonly MongoDBRepository<UserAvatar> _repo;
        private readonly IMapper _mapper;

        public UserAvatarService(MongoDBRepository<UserAvatar> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UserAvatarDto> CreateAsync(CreateUserAvatarDto dto)
        {
            var filter = Builders<UserAvatar>.Filter.Eq(x => x.UserOrAdminId, dto.UserOrAdminId);
            var existingList = await _repo.GetAllAsync(filter);
            var existing = existingList.FirstOrDefault();
            if (existing != null)
            {
                existing.AvatarCode = dto.AvatarCode;
                await _repo.UpdateAsync(existing.Id, existing);
                return _mapper.Map<UserAvatarDto>(existing);
            }
            var entity = _mapper.Map<UserAvatar>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<UserAvatarDto>(entity);
        }

        public async Task<IEnumerable<UserAvatarDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<UserAvatarDto>(x));
        }
        public async Task<UserAvatarDto?> GetByUserOrAdminIdAsync(Guid userOrAdminId)
        {
            var filter = Builders<UserAvatar>.Filter.Eq(x => x.UserOrAdminId, userOrAdminId);
            var list = await _repo.GetAllAsync(filter);
            var entity = list.FirstOrDefault();
            return entity == null ? null : _mapper.Map<UserAvatarDto>(entity);
        }

        public async Task<UserAvatarDto?> UpdateAsync(Guid userOrAdminId, UpdateUserAvatarDto dto)
        {
            var filter = Builders<UserAvatar>.Filter.Eq(x => x.UserOrAdminId, userOrAdminId);
            var list = await _repo.GetAllAsync(filter);
            var existing = list.FirstOrDefault();
            if (existing == null) return null;
            existing.AvatarCode = dto.AvatarCode;
            await _repo.UpdateAsync(existing.Id, existing);
            return _mapper.Map<UserAvatarDto>(existing);
        }
    }
}
