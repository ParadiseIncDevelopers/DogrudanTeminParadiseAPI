using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSProcurementEntryDocumentsService : IOSProcurementEntryDocumentsService
    {
        private readonly MongoDBRepository<OSProcurementEntryDocuments> _repo;
        private readonly GridFSRepository _fsRepo;
        private readonly IMapper _mapper;

        public OSProcurementEntryDocumentsService(
            MongoDBRepository<OSProcurementEntryDocuments> repo,
            GridFSRepository fsRepo,
            IMapper mapper)
        {
            _repo = repo;
            _fsRepo = fsRepo;
            _mapper = mapper;
        }

        public async Task<OSProcurementEntryDocumentsDto> CreateAsync(CreateOSProcurementEntryDocumentsDto dto)
        {
            var entity = new OSProcurementEntryDocuments
            {
                Id = Guid.NewGuid(),
                OneSourceProcurementEntryId = dto.OneSourceProcurementEntryId,
                TransactionAt = DateTime.UtcNow
            };
            foreach (var file in dto.EntrepriseFiles)
            {
                var fileId = await _fsRepo.UploadAsync(file, entity.Id + ".bin");
                entity.EntrepriseFileIds.Add(fileId);
            }
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSProcurementEntryDocumentsDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<OSProcurementEntryDocumentsDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = await _repo.GetAllAsync();
            return list.Where(d => d.OneSourceProcurementEntryId == entryId)
                       .Select(d => _mapper.Map<OSProcurementEntryDocumentsDto>(d));
        }

        public async Task<OSProcurementEntryDocumentsDto> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<OSProcurementEntryDocumentsDto>(entity);
        }

        public async Task<OSProcurementEntryDocumentsDto> UpdateAsync(Guid id, UpdateOSProcurementEntryDocumentsDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            entity.EntrepriseFileIds.Clear();
            foreach (var file in dto.EntrepriseFiles)
            {
                var fileId = await _fsRepo.UploadAsync(file, entity.Id + ".bin");
                entity.EntrepriseFileIds.Add(fileId);
            }
            await _repo.UpdateAsync(id, entity);
            return _mapper.Map<OSProcurementEntryDocumentsDto>(entity);
        }
    }
}
