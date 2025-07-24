using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSInspectionAcceptanceCertificateService : IOSInspectionAcceptanceCertificateService
    {
        private readonly MongoDBRepository<OSInspectionAcceptanceCertificate> _repo;
        private readonly IMapper _mapper;

        public OSInspectionAcceptanceCertificateService(
            MongoDBRepository<OSInspectionAcceptanceCertificate> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSInspectionAcceptanceCertificateDto> CreateAsync(CreateOSInspectionAcceptanceCertificateDto dto)
        {
            var entity = _mapper.Map<OSInspectionAcceptanceCertificate>(dto);
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
            return _mapper.Map<OSInspectionAcceptanceCertificateDto>(entity);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(_mapper.Map<OSInspectionAcceptanceCertificateDto>);
        }

        public async Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return Enumerable.Empty<OSInspectionAcceptanceCertificateDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(e => permittedEntryIds.Contains(e.OneSourceProcurementEntryId))
                       .Select(_mapper.Map<OSInspectionAcceptanceCertificateDto>);
        }

        public async Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(e => e.OneSourceProcurementEntryId == entryId);
            return list.Select(_mapper.Map<OSInspectionAcceptanceCertificateDto>);
        }

        public async Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null || !permittedEntryIds.Contains(entryId))
                return Enumerable.Empty<OSInspectionAcceptanceCertificateDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(e => e.OneSourceProcurementEntryId == entryId)
                       .Select(_mapper.Map<OSInspectionAcceptanceCertificateDto>);
        }

        public async Task<OSInspectionAcceptanceCertificateDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSInspectionAcceptanceCertificateDto>(e);
        }

        public async Task<OSInspectionAcceptanceCertificateDto?> UpdateAsync(Guid id, UpdateOSInspectionAcceptanceCertificateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            existing.SelectedProducts = dto.SelectedProducts.Select(i => new SelectedOfferItem
            {
                Id = i.Id == Guid.Empty ? Guid.NewGuid() : i.Id,
                Name = i.Name,
                Features = i.Features,
                Quantity = i.Quantity,
                UnitId = i.UnitId,
                UnitPrice = i.UnitPrice
            }).ToList();
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<OSInspectionAcceptanceCertificateDto>(existing);
        }
    }
}
