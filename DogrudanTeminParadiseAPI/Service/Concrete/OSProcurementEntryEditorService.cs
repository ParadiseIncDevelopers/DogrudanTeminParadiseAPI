using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSProcurementEntryEditorService : IOSProcurementEntryEditorService
    {
        private readonly MongoDBRepository<OSProcurementEntryEditor> _repo;
        private readonly IMapper _mapper;

        public OSProcurementEntryEditorService(MongoDBRepository<OSProcurementEntryEditor> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSProcurementEntryEditorDto> CreateAsync(CreateOSProcurementEntryEditorDto dto)
        {
            var entity = _mapper.Map<OSProcurementEntryEditor>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSProcurementEntryEditorDto>(entity);
        }

        public async Task<OSProcurementEntryEditorDto> GetAsync()
        {
            var e = (await _repo.GetAllAsync()).FirstOrDefault();
            return e == null ? null : _mapper.Map<OSProcurementEntryEditorDto>(e);
        }

        public async Task<OSProcurementEntryEditorDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSProcurementEntryEditorDto>(e);
        }

        public async Task<OSProcurementEntryEditorDto> UpdateAsync(UpdateOSProcurementEntryEditorDto dto)
        {
            var e = (await _repo.GetAllAsync()).FirstOrDefault(o => o.OneSourceProcurementEntryId == dto.OneSourceProcurementEntryId);
            if (e == null) return null;
            _mapper.Map(dto, e);
            await _repo.UpdateAsync(e.Id, e);
            return _mapper.Map<OSProcurementEntryEditorDto>(e);
        }

        public async Task<OSProcurementEntryEditorDto> GetEditorByEntryIdAsync(Guid entryId)
        {
            var all = await _repo.GetAllAsync();
            var editor = all.FirstOrDefault(o => o.OneSourceProcurementEntryId == entryId);
            return _mapper.Map<OSProcurementEntryEditorDto>(editor);
        }
    }
}
