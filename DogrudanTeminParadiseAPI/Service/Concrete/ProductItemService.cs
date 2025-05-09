using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ProductItemService : IProductItemService
    {
        private readonly MongoDBRepository<ProductItem> _repo;
        private readonly MongoDBRepository<Category> _catRepo;
        private readonly IMapper _mapper;

        public ProductItemService(MongoDBRepository<ProductItem> repo, MongoDBRepository<Category> catRepo, IMapper mapper)
        {
            _repo = repo;
            _catRepo = catRepo;
            _mapper = mapper;
        }

        public async Task<ProductItemDto> CreateAsync(CreateProductItemDto dto)
        {
            if (await _catRepo.GetByIdAsync(dto.CategoryId) == null)
                throw new KeyNotFoundException("Kategori bulunamadı.");
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Aynı isim veya koda sahip ürün kalemi mevcut.");
            var entity = _mapper.Map<ProductItem>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<ProductItemDto>(entity);
        }

        public async Task<IEnumerable<ProductItemDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<ProductItemDto>(x));
        }

        public async Task<ProductItemDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProductItemDto>(e);
        }

        public async Task<ProductItemDto> UpdateAsync(Guid id, UpdateProductItemDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            if (await _catRepo.GetByIdAsync(dto.CategoryId) == null)
                throw new KeyNotFoundException("Kategori bulunamadı.");
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Id != id && (x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException("Aynı isim veya koda sahip ürün kalemi mevcut.");
            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.CategoryId = dto.CategoryId;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<ProductItemDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Ürün kalemi bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
