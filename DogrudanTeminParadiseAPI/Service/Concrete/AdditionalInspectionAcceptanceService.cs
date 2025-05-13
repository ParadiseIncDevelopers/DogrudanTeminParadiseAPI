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
        private readonly IMapper _mapper;

        public AdditionalInspectionAcceptanceService(
            MongoDBRepository<AdditionalInspectionAcceptanceCertificate> repo,
            IMapper mapper)
        {
            _repo = repo;
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

        public async Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(c => c.ProcurementEntryId == entryId);
            return list.Select(c => _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(c));
        }

        public async Task<AdditionalInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<AdditionalInspectionAcceptanceCertificateDto>(e);
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

        public async Task DeleteAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
