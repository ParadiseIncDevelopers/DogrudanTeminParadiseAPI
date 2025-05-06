using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class TitleService : ITitleService
    {
        private readonly MongoDBRepository<Title> _repo;
        private readonly IMapper _mapper;

        public TitleService(MongoDBRepository<Title> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<TitleDto> CreateAsync(CreateTitleDto dto)
        {
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Bu isimde başka bir ünvan zaten mevcut.");
            var entity = _mapper.Map<Title>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<TitleDto>(entity);
        }

        public async Task<IEnumerable<TitleDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<TitleDto>(x));
        }

        public async Task<TitleDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<TitleDto>(e);
        }

        public async Task<TitleDto> UpdateAsync(Guid id, UpdateTitleDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            if (!existing.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var all = await _repo.GetAllAsync();
                if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Bu isimde başka bir ünvan zaten mevcut.");
            }
            existing.Name = dto.Name;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<TitleDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen ünvan bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
