using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ProcurementEntryService : IProcurementEntryService
    {
        private readonly MongoDBRepository<ProcurementEntry> _repo;
        private readonly MongoDBRepository<User> _userRepo;
        private readonly IMapper _mapper;

        public ProcurementEntryService(
            MongoDBRepository<ProcurementEntry> repo,
            MongoDBRepository<User> userRepo,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<ProcurementEntryDto> CreateAsync(CreateProcurementEntryDto dto)
        {
            var all = await _repo.GetAllAsync();

            if (all.Any(x => x.ProcurementDecisionNumber.Equals(dto.ProcurementDecisionNumber, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Bu satın alma karar numarası zaten mevcut.");

            foreach (var doc in dto.DocumentDatesAndNumbers)
            {
                bool existsDoc = all.Any(e => e.DocumentDatesAndNumbers.Any(d =>
                    d.Date == doc.Date && d.Number.Equals(doc.Number, StringComparison.OrdinalIgnoreCase)));
                if (existsDoc)
                    throw new InvalidOperationException($"Bu belge tarih ({doc.Date:d}) ve sayı ({doc.Number}) zaten kullanılmış.");
            }

            var user = await _userRepo.GetByIdAsync(dto.TenderResponsibleUserId);
            if (user == null)
                throw new KeyNotFoundException("İhale sorumlusu user bulunamadı.");

            var entity = _mapper.Map<ProcurementEntry>(dto);
            entity.Id = Guid.NewGuid();

            await _repo.InsertAsync(entity);
            return _mapper.Map<ProcurementEntryDto>(entity);
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<ProcurementEntryDto>(x));
        }

        public async Task<ProcurementEntryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProcurementEntryDto>(e);
        }

        public async Task<ProcurementEntryDto> UpdateAsync(Guid id, UpdateProcurementEntryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            var all = await _repo.GetAllAsync();

            if (!existing.ProcurementDecisionNumber.Equals(dto.ProcurementDecisionNumber, StringComparison.OrdinalIgnoreCase) &&
                all.Any(x => x.ProcurementDecisionNumber.Equals(dto.ProcurementDecisionNumber, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Bu satın alma karar numarası zaten mevcut.");

            foreach (var doc in dto.DocumentDatesAndNumbers)
            {
                bool existsDoc = all
                    .Where(x => x.Id != id)
                    .Any(e => e.DocumentDatesAndNumbers.Any(d =>
                        d.Date == doc.Date && d.Number.Equals(doc.Number, StringComparison.OrdinalIgnoreCase)));
                if (existsDoc)
                    throw new InvalidOperationException($"Bu belge tarih ({doc.Date:d}) ve sayı ({doc.Number}) zaten kullanılmış.");
            }

            var user = await _userRepo.GetByIdAsync(dto.TenderResponsibleUserId);
            if (user == null)
                throw new KeyNotFoundException("İhale sorumlusu user bulunamadı.");

            existing.ProcurementDecisionDate = dto.ProcurementDecisionDate;
            existing.ProcurementDecisionNumber = dto.ProcurementDecisionNumber;
            existing.TenderResponsibleUserId = dto.TenderResponsibleUserId;
            existing.TenderResponsibleTitle = dto.TenderResponsibleTitle;
            existing.WorkName = dto.WorkName;
            existing.WorkReason = dto.WorkReason;
            existing.BudgetAllocation = dto.BudgetAllocation;
            existing.SpecificationToBePrepared = dto.SpecificationToBePrepared;
            existing.ContractToBePrepared = dto.ContractToBePrepared;
            existing.DocumentDatesAndNumbers = dto.DocumentDatesAndNumbers;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<ProcurementEntryDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen Temin Girişi bulunamadı.");

            await _repo.DeleteAsync(id);
        }
    }
}
