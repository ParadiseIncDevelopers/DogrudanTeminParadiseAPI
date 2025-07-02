using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BudgetItemService : IBudgetItemService
    {
        private readonly MongoDBRepository<BudgetItem> _repo;
        private readonly IMapper _mapper;

        public BudgetItemService(MongoDBRepository<BudgetItem> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BudgetItemDto> CreateAsync(CreateBudgetItemDto dto)
        {
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Aynı isim veya kodda bütçe kalemi zaten mevcut.");
            var entity = _mapper.Map<BudgetItem>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<BudgetItemDto>(entity);
        }

        public async Task<IEnumerable<BudgetItemDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<BudgetItemDto>(x));
        }

        public async Task<BudgetItemDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BudgetItemDto>(e);
        }

        public async Task<BudgetItemDto> UpdateAsync(Guid id, UpdateBudgetItemDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Id != id && x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Aynı isim veya kodda bütçe kalemi zaten mevcut.");
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.ItemCode = dto.ItemCode;
            existing.EconomyCode = dto.EconomyCode;
            existing.FinancialCode = dto.FinancialCode;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<BudgetItemDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Bütçe kalemi bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
