using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using MongoDB.Driver;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class InspectionAcceptanceNoteService : IInspectionAcceptanceNoteService
    {
        private readonly MongoDBRepository<InspectionAcceptanceNote> _repo;
        private readonly IMapper _mapper;

        public InspectionAcceptanceNoteService(MongoDBRepository<InspectionAcceptanceNote> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<InspectionAcceptanceNoteDto> CreateAsync(CreateInspectionAcceptanceNoteDto dto)
        {
            var entity = _mapper.Map<InspectionAcceptanceNote>(dto);
            entity.Id = Guid.NewGuid().ToString();
            await _repo.InsertAsync(entity);
            return _mapper.Map<InspectionAcceptanceNoteDto>(entity);
        }

        public async Task<IEnumerable<InspectionAcceptanceNoteDto>> GetAllByEntryAsync(Guid procurementEntryId)
        {
            var filter = Builders<InspectionAcceptanceNote>.Filter.Eq(n => n.ProcurementEntryId, procurementEntryId.ToString());
            var list = await _repo.GetAllAsync(filter);
            return list.Select(_mapper.Map<InspectionAcceptanceNoteDto>);
        }

        public async Task<InspectionAcceptanceNoteDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<InspectionAcceptanceNoteDto>(e);
        }

        public async Task<InspectionAcceptanceNoteDto?> UpdateAsync(Guid id, UpdateInspectionAcceptanceNoteDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<InspectionAcceptanceNoteDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Note bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
