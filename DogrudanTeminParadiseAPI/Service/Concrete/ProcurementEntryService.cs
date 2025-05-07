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
            if (!string.IsNullOrEmpty(dto.ProcurementDecisionNumber) &&
                all.Any(x => x.ProcurementDecisionNumber.Equals(dto.ProcurementDecisionNumber, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Bu satın alma karar numarası zaten mevcut.");

            var user = await _userRepo.GetByIdAsync(dto.TenderResponsibleUserId);
            if (user == null)
                throw new KeyNotFoundException("İhale sorumlusu user bulunamadı.");

            var entity = _mapper.Map<ProcurementEntry>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<ProcurementEntryDto>(entity);
        }

        public async Task<IEnumerable<ProcurementEntryDto>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(x => _mapper.Map<ProcurementEntryDto>(x));

        public async Task<ProcurementEntryDto> GetByIdAsync(Guid id)
            => (await _repo.GetByIdAsync(id)) is var e && e != null ? _mapper.Map<ProcurementEntryDto>(e) : null;

        public async Task<ProcurementEntryDto> UpdateAsync(Guid id, UpdateProcurementEntryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Yeni karar numarası
            var newNumber = dto.ProcurementDecisionNumber ?? string.Empty;

            // Eğer kullanıcı bir karar numarası göndermişse ve bu numara mevcut olandan farklıysa...
            if (!string.IsNullOrWhiteSpace(newNumber)
                && !string.Equals(existing.ProcurementDecisionNumber, newNumber, StringComparison.OrdinalIgnoreCase))
            {
                // ...ve başka bir kayıtta zaten aynı numara varsa
                var all = await _repo.GetAllAsync();
                if (all.Any(x => string.Equals(x.ProcurementDecisionNumber, newNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Bu satın alma karar numarası zaten mevcut.");
                }
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
            existing.PiyasaArastirmaOnayDate = dto.PiyasaArastirmaOnayDate;
            existing.PiyasaArastirmaOnayNumber = dto.PiyasaArastirmaOnayNumber;
            existing.TeklifMektubuDate = dto.TeklifMektubuDate;
            existing.TeklifMektubuNumber = dto.TeklifMektubuNumber;
            existing.PiyasaArastirmaBaslangicDate = dto.PiyasaArastirmaBaslangicDate;
            existing.PiyasaArastirmaBaslangicNumber = dto.PiyasaArastirmaBaslangicNumber;
            existing.YaklasikMaliyetHesaplamaBaslangicDate = dto.YaklasikMaliyetHesaplamaBaslangicDate;
            existing.YaklasikMaliyetHesaplamaBaslangicNumber = dto.YaklasikMaliyetHesaplamaBaslangicNumber;
            existing.MuayeneVeKabulBelgesiDate = dto.MuayeneVeKabulBelgesiDate;
            existing.MuayeneVeKabulBelgesiNumber = dto.MuayeneVeKabulBelgesiNumber;
            existing.AdministrationUnitId = dto.AdministrationUnitId;
            existing.SubAdministrationUnitId = dto.SubAdministrationUnitId;
            existing.ThreeSubAdministrationUnitId = dto.ThreeSubAdministrationUnitId;

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
