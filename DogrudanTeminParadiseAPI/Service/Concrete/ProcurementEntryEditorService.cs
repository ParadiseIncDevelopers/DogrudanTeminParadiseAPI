using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ProcurementEntryEditorService : IProcurementEntryEditorService
    {
        private readonly MongoDBRepository<ProcurementEntryEditor> _repo;
        private readonly IMapper _mapper;

        public ProcurementEntryEditorService(
            MongoDBRepository<ProcurementEntryEditor> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ProcurementEntryEditorDto> CreateAsync(CreateProcurementEntryEditorDto dto)
        {
            if ((await _repo.GetAllAsync()).Any())
                throw new InvalidOperationException("Sadece bir editör olabilir.");
            var entity = _mapper.Map<ProcurementEntryEditor>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<ProcurementEntryEditorDto>(entity);
        }

        public async Task<ProcurementEntryEditorDto> GetAsync()
        {
            var e = (await _repo.GetAllAsync()).FirstOrDefault();
            return e == null ? null : _mapper.Map<ProcurementEntryEditorDto>(e);
        }

        public async Task<ProcurementEntryEditorDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ProcurementEntryEditorDto>(e);
        }

        public async Task<ProcurementEntryEditorDto> UpdateAsync(UpdateProcurementEntryEditorDto dto)
        {
            var e = (await _repo.GetAllAsync()).FirstOrDefault();
            if (e == null) return null;
            _mapper.Map(dto, e);
            await _repo.UpdateAsync(e.Id, e);
            return _mapper.Map<ProcurementEntryEditorDto>(e);
        }

        public async Task<ProcurementEntryEditorDto> GetEditorByEntryIdAsync(Guid entryId)
        {
            // Tüm teklif mektuplarını alıp, ilgili entryId ile filtreliyoruz
            var allOffers = await _repo.GetAllAsync();
            var offers = allOffers.FirstOrDefault(o => o.ProcurementEntryId == entryId);
            // Modelden DTO’ya çevir
            return _mapper.Map<ProcurementEntryEditorDto>(offers);
        }
    }
}
