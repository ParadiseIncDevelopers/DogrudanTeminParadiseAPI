using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class DecisionNumbersService : IDecisionNumbersService
    {
        private readonly MongoDBRepository<DecisionNumbers> _repo;
        private readonly IMapper _mapper;

        public DecisionNumbersService(MongoDBRepository<DecisionNumbers> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<DecisionNumbersDto> CreateAsync(CreateDecisionNumbersDto dto)
        {
            var entity = _mapper.Map<DecisionNumbers>(dto);
            entity.Id = Guid.NewGuid().ToString();
            await _repo.InsertAsync(entity);
            return _mapper.Map<DecisionNumbersDto>(entity);
        }

        public async Task<IEnumerable<DecisionNumbersDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(_mapper.Map<DecisionNumbersDto>);
        }

        public async Task<DecisionNumbersDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<DecisionNumbersDto>(e);
        }

        public async Task<DecisionNumbersDto?> UpdateAsync(Guid id, UpdateDecisionNumbersDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;
            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<DecisionNumbersDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("DecisionNumbers not found.");
            await _repo.DeleteAsync(id);
        }
    }
}
