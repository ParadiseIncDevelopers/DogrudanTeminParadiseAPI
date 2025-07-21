using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ProductService : IProductService
    {
        private readonly MongoDBRepository<Product> _prodRepo;
        private readonly MongoDBRepository<Category> _catRepo;
        private readonly MongoDBRepository<ProductItem> _itemRepo;
        private readonly IMapper _mapper;

        public ProductService(
            MongoDBRepository<Product> prodRepo,
            MongoDBRepository<Category> catRepo,
            MongoDBRepository<ProductItem> itemRepo,
            IMapper mapper)
        {
            _prodRepo = prodRepo;
            _catRepo = catRepo;
            _itemRepo = itemRepo;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            if (await _catRepo.GetByIdAsync(dto.CategoryId) == null)
                throw new KeyNotFoundException("Kategori bulunamadı.");
            if (await _itemRepo.GetByIdAsync(dto.ProductItemId) == null)
                throw new KeyNotFoundException("Ürün kalemi bulunamadı.");
            var all = await _prodRepo.GetAllAsync();
            if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Aynı isim veya koda sahip ürün mevcut.");
            var entity = _mapper.Map<Product>(dto);
            entity.Id = Guid.NewGuid();
            await _prodRepo.InsertAsync(entity);
            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<IEnumerable<ProductDto>> AddMassAsync(List<CreateProductDto> dtos)
        {
            var all = (await _prodRepo.GetAllAsync()).ToList();
            var entities = new List<Product>();
            foreach (var dto in dtos)
            {
                if (await _catRepo.GetByIdAsync(dto.CategoryId) == null)
                    throw new KeyNotFoundException("Kategori bulunamadı.");
                if (await _itemRepo.GetByIdAsync(dto.ProductItemId) == null)
                    throw new KeyNotFoundException("Ürün kalemi bulunamadı.");
                if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)) ||
                    entities.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Aynı isim veya koda sahip ürün mevcut.");
                var entity = _mapper.Map<Product>(dto);
                entity.Id = Guid.NewGuid();
                entities.Add(entity);
                all.Add(entity);
            }
            await _prodRepo.InsertManyAsync(entities);
            return entities.Select(x => _mapper.Map<ProductDto>(x));
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var list = await _prodRepo.GetAllAsync();
            return list.Select(x => _mapper.Map<ProductDto>(x));
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            var e = await _prodRepo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProductDto>(e);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var existing = await _prodRepo.GetByIdAsync(id);
            if (existing == null) return null;
            if (await _catRepo.GetByIdAsync(dto.CategoryId) == null)
                throw new KeyNotFoundException("Kategori bulunamadı.");
            if (await _itemRepo.GetByIdAsync(dto.ProductItemId) == null)
                throw new KeyNotFoundException("Ürün kalemi bulunamadı.");
            var all = await _prodRepo.GetAllAsync();
            if (all.Any(x => x.Id != id && (x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) || x.Code.Equals(dto.Code, StringComparison.OrdinalIgnoreCase))))
                throw new InvalidOperationException("Aynı isim veya koda sahip ürün mevcut.");
            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.CategoryId = dto.CategoryId;
            existing.ProductItemId = dto.ProductItemId;
            await _prodRepo.UpdateAsync(id, existing);
            return _mapper.Map<ProductDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _prodRepo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Ürün bulunamadı.");
            await _prodRepo.DeleteAsync(id);
        }
    }
}
