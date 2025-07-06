using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BackupAdditionalInspectionAcceptanceService : IBackupAdditionalInspectionAcceptanceService
    {
        private readonly MongoDBRepository<BackupAdditionalInspectionAcceptanceCertificate> _repo;
        private readonly IMapper _mapper;

        public BackupAdditionalInspectionAcceptanceService(MongoDBRepository<BackupAdditionalInspectionAcceptanceCertificate> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BackupAdditionalInspectionAcceptanceCertificateDto> CreateAsync(CreateBackupAdditionalInspectionAcceptanceCertificateDto dto)
        {
            var entity = _mapper.Map<BackupAdditionalInspectionAcceptanceCertificate>(dto);
            entity.Id = Guid.NewGuid();
            entity.SelectedProducts = dto.SelectedProducts.Select(i => new SelectedOfferItem
            {
                Id = Guid.NewGuid(),
                Name = i.Name,
                Features = i.Features,
                Quantity = i.Quantity,
                UnitId = i.UnitId,
                UnitPrice = i.UnitPrice
            }).ToList();
            await _repo.InsertAsync(entity);
            return _mapper.Map<BackupAdditionalInspectionAcceptanceCertificateDto>(entity);
        }

        public async Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => _mapper.Map<BackupAdditionalInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return Enumerable.Empty<BackupAdditionalInspectionAcceptanceCertificateDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(e => permittedEntryIds.Contains(e.ProcurementEntryId)).Select(e => _mapper.Map<BackupAdditionalInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = await _repo.GetAllAsync();
            return list.Where(e => e.ProcurementEntryId == entryId).Select(e => _mapper.Map<BackupAdditionalInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null || !permittedEntryIds.Contains(entryId))
                return Enumerable.Empty<BackupAdditionalInspectionAcceptanceCertificateDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(e => e.ProcurementEntryId == entryId).Select(e => _mapper.Map<BackupAdditionalInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<BackupAdditionalInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BackupAdditionalInspectionAcceptanceCertificateDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
