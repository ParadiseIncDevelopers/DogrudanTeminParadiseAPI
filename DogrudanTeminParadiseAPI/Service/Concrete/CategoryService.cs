using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly MongoDBRepository<Category> _repo;
        private readonly IMapper _mapper;

        public CategoryService(MongoDBRepository<Category> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Aynı isim veya koda sahip kategori zaten mevcut.");
            var entity = _mapper.Map<Category>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<CategoryDto>(x));
        }

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<CategoryDto>(e);
        }

        public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Id != id && (x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException("Aynı isim veya koda sahip kategori zaten mevcut.");
            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<CategoryDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Kategori bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
