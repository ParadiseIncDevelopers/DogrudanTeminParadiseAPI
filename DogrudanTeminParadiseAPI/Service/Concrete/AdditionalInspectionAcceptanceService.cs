using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class AdditionalInspectionAcceptanceService : IAdditionalInspectionAcceptanceService
    {
        private readonly MongoDBRepository<AdditionalInspectionAcceptanceCertificate> _repo;
        private readonly MongoDBRepository<BackupAdditionalInspectionAcceptanceCertificate> _backupRepo;
        private readonly IMapper _mapper;

        public AdditionalInspectionAcceptanceService(
            MongoDBRepository<AdditionalInspectionAcceptanceCertificate> repo,
            MongoDBRepository<BackupAdditionalInspectionAcceptanceCertificate> backupRepo,
            IMapper mapper)
        {
            _repo = repo;
            _backupRepo = backupRepo;
            _mapper = mapper;
        }

        public async Task<AdditionalInspectionAcceptanceCertificateDto> CreateAsync(CreateAdditionalInspectionAcceptanceCertificateDto dto)
        {
            var entity = _mapper.Map<AdditionalInspectionAcceptanceCertificate>(dto);
            entity.Id = Guid.NewGuid();
            entity.SelectedProducts = dto.SelectedProducts
                .Select(i => new SelectedOfferItem
                {
                    Id = Guid.NewGuid(),
                    Name = i.Name,
                    Features = i.Features,
                    Quantity = i.Quantity,
                    UnitId = i.UnitId,
                    UnitPrice = i.UnitPrice
                }).ToList();
            await _repo.InsertAsync(entity);
            return _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(entity);
        }

        public async Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return Enumerable.Empty<AdditionalInspectionAcceptanceCertificateDto>();

            var list = await _repo.GetAllAsync();
            return list
                .Where(e => permittedEntryIds.Contains(e.ProcurementEntryId))
                .Select(e => _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = await _repo.GetAllAsync();
            return list
                .Where(e => e.ProcurementEntryId == entryId)
                .Select(e => _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null || !permittedEntryIds.Contains(entryId))
                return Enumerable.Empty<AdditionalInspectionAcceptanceCertificateDto>();

            var list = await _repo.GetAllAsync();
            return list
                .Where(e => e.ProcurementEntryId == entryId)
                .Select(e => _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<AdditionalInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(e);
        }

        public async Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllAsync()
        {
            var e = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>>(e);
        }

        public async Task<AdditionalInspectionAcceptanceCertificateDto> UpdateAsync(Guid id, UpdateAdditionalInspectionAcceptanceCertificateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            existing.SelectedProducts = dto.SelectedProducts
                .Select(i => new SelectedOfferItem
                {
                    Id = i.Id == Guid.Empty ? Guid.NewGuid() : i.Id,
                    Name = i.Name,
                    Features = i.Features,
                    Quantity = i.Quantity,
                    UnitId = i.UnitId,
                    UnitPrice = i.UnitPrice
                }).ToList();
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(existing);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            var backup = _mapper.Map<BackupAdditionalInspectionAcceptanceCertificate>(e);
            backup.RemovedByUserId = userId;
            backup.RemovingDate = DateTime.UtcNow;
            await _backupRepo.InsertAsync(backup);
            await _repo.DeleteAsync(id);
        }
    }
}
