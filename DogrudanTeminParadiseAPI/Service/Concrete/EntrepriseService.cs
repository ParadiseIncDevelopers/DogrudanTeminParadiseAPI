using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class EntrepriseService : IEntrepriseService
    {
        private readonly MongoDBRepository<Entreprise> _repo;
        private readonly IMapper _mapper;

        public EntrepriseService(MongoDBRepository<Entreprise> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EntrepriseDto> CreateAsync(CreateEntrepriseDto dto)
        {
            var allEntreprises = await _repo.GetAllAsync();

            // VKN eşleşme kontrolü
            bool vknExists = allEntreprises.Any(e => e.Vkn == dto.Vkn);

            if (vknExists)
                throw new InvalidOperationException("Bu VKN numarasına sahip bir firma zaten mevcut.");

            // UNVAN eşleşme kontrolü (büyük küçük harf duyarsız)
            bool unvanExists = allEntreprises.Any(e => e.Unvan.Equals(dto.Unvan, StringComparison.OrdinalIgnoreCase));

            if (unvanExists)
                throw new InvalidOperationException("Bu Ünvan (firma adı) başka bir firma tarafından zaten kullanılıyor.");

            var entity = _mapper.Map<Entreprise>(dto);
            entity.Id = Guid.NewGuid();

            await _repo.InsertAsync(entity);
            return _mapper.Map<EntrepriseDto>(entity);
        }

        public async Task<IEnumerable<EntrepriseDto>> GetAllAsync()
        {
            var entreprises = await _repo.GetAllAsync();
            return entreprises.Select(e => _mapper.Map<EntrepriseDto>(e));
        }

        public async Task<EntrepriseDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<EntrepriseDto>(e);
        }

        public async Task<EntrepriseDto> UpdateAsync(Guid id, UpdateEntrepriseDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Güncellenecek alanlar
            existing.Vkn = dto.Vkn;
            existing.Unvan = dto.Unvan;
            existing.NaceKodu = dto.NaceKodu;
            existing.FirmaYetkilisi = dto.FirmaYetkilisi;
            existing.CalisanSayisi = dto.CalisanSayisi;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<EntrepriseDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen firma bulunamadı.");

            await _repo.DeleteAsync(id);
        }
    }
}
