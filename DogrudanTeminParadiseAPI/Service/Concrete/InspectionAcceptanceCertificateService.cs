﻿using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using SharpCompress.Common;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class InspectionAcceptanceCertificateService : IInspectionAcceptanceCertificateService
    {
        private readonly MongoDBRepository<InspectionAcceptanceCertificate> _repo;
        private readonly MongoDBRepository<BackupInspectionAcceptanceCertificate> _backupRepo;
        private readonly IMapper _mapper;

        public InspectionAcceptanceCertificateService(
            MongoDBRepository<InspectionAcceptanceCertificate> repo,
            MongoDBRepository<BackupInspectionAcceptanceCertificate> backupRepo,
            IMapper mapper)
        {
            _repo = repo;
            _backupRepo = backupRepo;
            _mapper = mapper;
        }

        public async Task<InspectionAcceptanceCertificateDto> CreateAsync(CreateInspectionAcceptanceCertificateDto dto)
        {
            try
            {
                var entity = _mapper.Map<InspectionAcceptanceCertificate>(dto);
                entity.Id = Guid.NewGuid();
                // Map SelectedProducts from CreateOfferItemDto to SelectedOfferItem model
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
                return _mapper.Map<InspectionAcceptanceCertificateDto>(entity);
            }
            catch (Exception e) 
            {
                return _mapper.Map<InspectionAcceptanceCertificateDto>(new InspectionAcceptanceCertificateDto());
            }
        }

        public async Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(e => e.ProcurementEntryId == entryId);
            return list.Select(e => _mapper.Map<InspectionAcceptanceCertificateDto>(e));
        }

        public async Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null || !permittedEntryIds.Contains(entryId))
                return Enumerable.Empty<InspectionAcceptanceCertificateDto>();

            var list = await _repo.GetAllAsync();
            return list
                .Where(e => e.ProcurementEntryId == entryId)
                .Select(e => _mapper.Map<InspectionAcceptanceCertificateDto>(e));
        }

        public async Task<InspectionAcceptanceCertificateDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<InspectionAcceptanceCertificateDto>(e);
        }

        public async Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<InspectionAcceptanceCertificateDto>>(list);
        }

        public async Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds)
        {
            if (permittedEntryIds == null)
                return [];

            var list = await _repo.GetAllAsync();
            return list
                .Where(e => permittedEntryIds.Contains(e.ProcurementEntryId))
                .Select(e => _mapper.Map<InspectionAcceptanceCertificateDto>(e));
        }

        public async Task<InspectionAcceptanceCertificateDto> UpdateAsync(Guid id, UpdateInspectionAcceptanceCertificateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            existing.SelectedProducts = dto.SelectedProducts
                .Select(i => new SelectedOfferItem
                {
                    Id = i.Id == Guid.Empty ? Guid.NewGuid() : i.Id,
                    Features = i.Features,
                    Quantity = i.Quantity,
                    UnitId = i.UnitId,
                    UnitPrice = i.UnitPrice,
                    Name = i.Name
                }).ToList();
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<InspectionAcceptanceCertificateDto>(existing);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            var backup = _mapper.Map<BackupInspectionAcceptanceCertificate>(e);
            backup.RemovedByUserId = userId;
            backup.RemovingDate = DateTime.UtcNow;
            await _backupRepo.InsertAsync(backup);
            await _repo.DeleteAsync(id);
        }
    }
}
