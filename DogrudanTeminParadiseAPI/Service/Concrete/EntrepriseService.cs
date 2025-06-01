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

            bool vknExists = allEntreprises.Any(e => e.Vkn == dto.Vkn);
            if (vknExists)
                throw new InvalidOperationException("Bu VKN numarasına sahip bir firma zaten mevcut.");

            bool unvanExists = allEntreprises.Any(e =>
                e.Unvan.Equals(dto.Unvan, StringComparison.OrdinalIgnoreCase));
            if (unvanExists)
                throw new InvalidOperationException("Bu Ünvan (firma adı) başka bir firma tarafından zaten kullanılıyor.");

            bool addressExists = allEntreprises.Any(e =>
                e.Address.Equals(dto.Address, StringComparison.OrdinalIgnoreCase));
            if (addressExists)
                throw new InvalidOperationException("Bu Adres başka bir firma tarafından zaten kullanılıyor.");

            bool emailExists = allEntreprises.Any(e =>
                e.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
            if (emailExists)
                throw new InvalidOperationException("Bu e-posta başka bir firma tarafından zaten kullanılıyor.");

            bool phoneExists = allEntreprises.Any(e =>
                e.PhoneNumber.Equals(dto.PhoneNumber, StringComparison.OrdinalIgnoreCase));
            if (phoneExists)
                throw new InvalidOperationException("Bu telefon numarası başka bir firma tarafından zaten kullanılıyor.");

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

            var allEntreprises = await _repo.GetAllAsync();

            bool vknExists = allEntreprises
                .Where(e => e.Id != id)
                .Any(e => e.Vkn == dto.Vkn);
            if (vknExists)
                throw new InvalidOperationException("Bu VKN numarasına sahip bir firma zaten mevcut.");

            bool unvanExists = allEntreprises
                .Where(e => e.Id != id)
                .Any(e => e.Unvan.Equals(dto.Unvan, StringComparison.OrdinalIgnoreCase));
            if (unvanExists)
                throw new InvalidOperationException("Bu Ünvan (firma adı) başka bir firma tarafından zaten kullanılıyor.");

            bool addressExists = allEntreprises
                .Where(e => e.Id != id)
                .Any(e => e.Address.Equals(dto.Address, StringComparison.OrdinalIgnoreCase));
            if (addressExists)
                throw new InvalidOperationException("Bu Adres başka bir firma tarafından zaten kullanılıyor.");

            bool emailExists = allEntreprises
                .Where(e => e.Id != id)
                .Any(e => e.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
            if (emailExists)
                throw new InvalidOperationException("Bu e-posta başka bir firma tarafından zaten kullanılıyor.");

            bool phoneExists = allEntreprises
                .Where(e => e.Id != id)
                .Any(e => e.PhoneNumber.Equals(dto.PhoneNumber, StringComparison.OrdinalIgnoreCase));
            if (phoneExists)
                throw new InvalidOperationException("Bu telefon numarası başka bir firma tarafından zaten kullanılıyor.");

            existing.Unvan = dto.Unvan;
            existing.Vkn = dto.Vkn;
            existing.NaceKodu = dto.NaceKodu;
            existing.FirmaYetkilisi = dto.FirmaYetkilisi;
            existing.CalisanSayisi = dto.CalisanSayisi;

            existing.Address = dto.Address;
            existing.TaxOffice = dto.TaxOffice;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;

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
