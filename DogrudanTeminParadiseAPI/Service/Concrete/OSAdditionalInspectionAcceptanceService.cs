using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSAdditionalInspectionAcceptanceService : IOSAdditionalInspectionAcceptanceService
    {
        private readonly MongoDBRepository<OSAdditionalInspectionAcceptanceCertificate> _repo;
        private readonly IMapper _mapper;

        public OSAdditionalInspectionAcceptanceService(
            MongoDBRepository<OSAdditionalInspectionAcceptanceCertificate> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSAdditionalInspectionAcceptanceCertificateDto> CreateAsync(CreateOSAdditionalInspectionAcceptanceCertificateDto dto)
        {
            var entity = _mapper.Map<OSAdditionalInspectionAcceptanceCertificate>(dto);
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
            return _mapper.Map<OSAdditionalInspectionAcceptanceCertificateDto>(entity);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(_mapper.Map<OSAdditionalInspectionAcceptanceCertificateDto>);
        }

        public async Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return Enumerable.Empty<OSAdditionalInspectionAcceptanceCertificateDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(e => permittedEntryIds.Contains(e.OneSourceProcurementEntryId))
                       .Select(_mapper.Map<OSAdditionalInspectionAcceptanceCertificateDto>);
        }

        public async Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(e => e.OneSourceProcurementEntryId == entryId);
            return list.Select(_mapper.Map<OSAdditionalInspectionAcceptanceCertificateDto>);
        }

        public async Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null || !permittedEntryIds.Contains(entryId))
                return Enumerable.Empty<OSAdditionalInspectionAcceptanceCertificateDto>();
            var list = await _repo.GetAllAsync();
            return list.Where(e => e.OneSourceProcurementEntryId == entryId)
                       .Select(_mapper.Map<OSAdditionalInspectionAcceptanceCertificateDto>);
        }

        public async Task<OSAdditionalInspectionAcceptanceCertificateDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSAdditionalInspectionAcceptanceCertificateDto>(e);
        }

        public async Task<OSAdditionalInspectionAcceptanceCertificateDto?> UpdateAsync(Guid id, UpdateOSAdditionalInspectionAcceptanceCertificateDto dto)
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
            return _mapper.Map<OSAdditionalInspectionAcceptanceCertificateDto>(existing);
        }
    }
}
