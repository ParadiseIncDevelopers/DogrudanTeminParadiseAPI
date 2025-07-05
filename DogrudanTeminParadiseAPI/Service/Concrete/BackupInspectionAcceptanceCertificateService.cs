using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BackupInspectionAcceptanceCertificateService : IBackupInspectionAcceptanceCertificateService
    {
        private readonly MongoDBRepository<BackupInspectionAcceptanceCertificate> _repo;
        private readonly IMapper _mapper;

        public BackupInspectionAcceptanceCertificateService(MongoDBRepository<BackupInspectionAcceptanceCertificate> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BackupInspectionAcceptanceCertificateDto> CreateAsync(CreateBackupInspectionAcceptanceCertificateDto dto)
        {
            var entity = _mapper.Map<BackupInspectionAcceptanceCertificate>(dto);
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
            return _mapper.Map<BackupInspectionAcceptanceCertificateDto>(entity);
        }

        public async Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => _mapper.Map<BackupInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return [];
            var list = await _repo.GetAllAsync();
            return list.Where(e => permittedEntryIds.Contains(e.ProcurementEntryId)).Select(e => _mapper.Map<BackupInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync()).Where(e => e.ProcurementEntryId == entryId);
            return list.Select(e => _mapper.Map<BackupInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null || !permittedEntryIds.Contains(entryId))
                return Enumerable.Empty<BackupInspectionAcceptanceCertificateDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(e => e.ProcurementEntryId == entryId).Select(e => _mapper.Map<BackupInspectionAcceptanceCertificateDto>(e));
        }

        public async Task<BackupInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BackupInspectionAcceptanceCertificateDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
