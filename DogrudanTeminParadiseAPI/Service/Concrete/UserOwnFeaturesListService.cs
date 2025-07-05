using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using MongoDB.Driver;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class UserOwnFeaturesListService : IUserOwnFeaturesListService
    {
        private readonly MongoDBRepository<UserOwnFeaturesList> _repo;
        private readonly IMapper _mapper;

        public UserOwnFeaturesListService(
            MongoDBRepository<UserOwnFeaturesList> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UserOwnFeaturesListDto> CreateAsync(CreateUserOwnFeaturesListDto dto)
        {
            var entity = _mapper.Map<UserOwnFeaturesList>(dto);
            entity.Id = Guid.NewGuid().ToString();
            // Her nested UserFeatures için yeni Guid atıyoruz
            foreach (var uf in entity.FeaturesLists)
                uf.Id = Guid.NewGuid().ToString();

            await _repo.InsertAsync(entity);
            return _mapper.Map<UserOwnFeaturesListDto>(entity);
        }

        public async Task<IEnumerable<UserOwnFeaturesListDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => _mapper.Map<UserOwnFeaturesListDto>(e));
        }

        public async Task<IEnumerable<UserOwnFeaturesListDto>> GetByUserIdAsync(Guid userId)
        {
            var filter = Builders<UserOwnFeaturesList>
                .Filter.Eq(x => x.UserId, userId.ToString());
            var list = await _repo.GetAllAsync(filter);
            return list.Select(e => _mapper.Map<UserOwnFeaturesListDto>(e));
        }

        public async Task<UserOwnFeaturesListDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null
                ? null
                : _mapper.Map<UserOwnFeaturesListDto>(e);
        }

        public async Task<UserOwnFeaturesListDto> UpdateAsync(Guid id, UpdateUserOwnFeaturesListDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            // Replace only the FeaturesLists
            var updatedLists = _mapper.Map<List<UserFeatures>>(dto.FeaturesLists);
            existing.FeaturesLists = updatedLists;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<UserOwnFeaturesListDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Özellik listesi bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
