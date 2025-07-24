using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using MongoDB.Driver;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSInspectionAcceptanceNoteService : IOSInspectionAcceptanceNoteService
    {
        private readonly MongoDBRepository<OSInspectionAcceptanceNote> _repo;
        private readonly IMapper _mapper;

        public OSInspectionAcceptanceNoteService(MongoDBRepository<OSInspectionAcceptanceNote> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSInspectionAcceptanceNoteDto> CreateAsync(CreateOSInspectionAcceptanceNoteDto dto)
        {
            var entity = _mapper.Map<OSInspectionAcceptanceNote>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSInspectionAcceptanceNoteDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Note bulunamadı.");
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<OSInspectionAcceptanceNoteDto>> GetAllByEntryAsync(Guid entryId)
        {
            var filter = Builders<OSInspectionAcceptanceNote>.Filter.Eq(n => n.OneSourceProcurementEntryId, entryId);
            var list = await _repo.GetAllAsync(filter);
            return list.Select(_mapper.Map<OSInspectionAcceptanceNoteDto>);
        }

        public async Task<OSInspectionAcceptanceNoteDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSInspectionAcceptanceNoteDto>(e);
        }

        public async Task<OSInspectionAcceptanceNoteDto?> UpdateAsync(Guid id, UpdateOSInspectionAcceptanceNoteDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<OSInspectionAcceptanceNoteDto>(existing);
        }
    }
}
