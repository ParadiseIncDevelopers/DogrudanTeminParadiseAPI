using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ReportBudgetItemService : IReportBudgetItemService
    {
        private readonly MongoDBRepository<ProcurementEntry> _entryRepo;
        private readonly MongoDBRepository<BudgetItem> _budgetRepo;
        private readonly MongoDBRepository<InspectionAcceptanceCertificate> _inspectionRepo;
        private readonly MongoDBRepository<AdditionalInspectionAcceptanceCertificate> _addInspectionRepo;
        private readonly MongoDBRepository<OfferLetter> _offerRepo;
        private readonly MongoDBRepository<User> _userRepo;
        private readonly MongoDBRepository<AdminUser> _adminRepo;

        public ReportBudgetItemService(
            MongoDBRepository<ProcurementEntry> entryRepo,
            MongoDBRepository<BudgetItem> budgetRepo,
            MongoDBRepository<InspectionAcceptanceCertificate> inspectionRepo,
            MongoDBRepository<AdditionalInspectionAcceptanceCertificate> addInspectionRepo,
            MongoDBRepository<OfferLetter> offerRepo,
            MongoDBRepository<User> userRepo,
            MongoDBRepository<AdminUser> adminRepo)
        {
            _entryRepo = entryRepo;
            _budgetRepo = budgetRepo;
            _inspectionRepo = inspectionRepo;
            _addInspectionRepo = addInspectionRepo;
            _offerRepo = offerRepo;
            _userRepo = userRepo;
            _adminRepo = adminRepo;
        }

        private async Task<Dictionary<Guid, BudgetItem>> GetBudgetLookupAsync()
        {
            var list = await _budgetRepo.GetAllAsync();
            return list.ToDictionary(b => b.Id);
        }

        public async Task<IEnumerable<BudgetItemCountDto>> GetMostEntryBudgetItemsAsync(int top = 3)
        {
            var entries = await _entryRepo.GetAllAsync();
            var lookup = await GetBudgetLookupAsync();

            var counts = entries
                .Where(e => e.BudgetAllocationId.HasValue)
                .GroupBy(e => e.BudgetAllocationId!.Value)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var result = new List<BudgetItemCountDto>();
            foreach (var item in topList)
            {
                if (lookup.TryGetValue(item.Id, out var bi))
                {
                    result.Add(new BudgetItemCountDto
                    {
                        BudgetName = bi.Name,
                        EconomyCode = bi.EconomyCode,
                        FinancialCode = bi.FinancialCode,
                        Count = item.Count
                    });
                }
            }
            if (other > 0)
            {
                result.Add(new BudgetItemCountDto
                {
                    BudgetName = "Diğer",
                    Count = other
                });
            }
            return result;
        }

        public async Task<IEnumerable<BudgetItemCountDto>> GetLeastEntryBudgetItemsAsync(int top = 3)
        {
            var entries = await _entryRepo.GetAllAsync();
            var lookup = await GetBudgetLookupAsync();

            var counts = entries
                .Where(e => e.BudgetAllocationId.HasValue)
                .GroupBy(e => e.BudgetAllocationId!.Value)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderBy(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var result = new List<BudgetItemCountDto>();
            foreach (var item in topList)
            {
                if (lookup.TryGetValue(item.Id, out var bi))
                {
                    result.Add(new BudgetItemCountDto
                    {
                        BudgetName = bi.Name,
                        EconomyCode = bi.EconomyCode,
                        FinancialCode = bi.FinancialCode,
                        Count = item.Count
                    });
                }
            }
            if (other > 0)
            {
                result.Add(new BudgetItemCountDto
                {
                    BudgetName = "Diğer",
                    Count = other
                });
            }
            return result;
        }

        public async Task<IEnumerable<UserBudgetItemCountDto>> GetUserEntryExtremesAsync()
        {
            var entries = (await _entryRepo.GetAllAsync())
                .Where(e => e.TenderResponsibleUserId.HasValue && e.BudgetAllocationId.HasValue)
                .ToList();

            var budgets = await GetBudgetLookupAsync();
            var admins = (await _adminRepo.GetAllAsync()).ToDictionary(a => a.Id);
            var users = (await _userRepo.GetAllAsync()).ToDictionary(u => u.Id);

            var result = new List<UserBudgetItemCountDto>();

            var groupedByUser = entries.GroupBy(e => e.TenderResponsibleUserId!.Value);
            foreach (var grp in groupedByUser)
            {
                string name = "Bilinmeyen";
                if (admins.TryGetValue(grp.Key, out var adm))
                    name = $"{Crypto.Decrypt(adm.Name)} {Crypto.Decrypt(adm.Surname)}";
                else if (users.TryGetValue(grp.Key, out var usr))
                    name = $"{Crypto.Decrypt(usr.Name)} {Crypto.Decrypt(usr.Surname)}";

                var itemCounts = grp.GroupBy(e => e.BudgetAllocationId!.Value)
                    .Select(g => new { Id = g.Key, Count = g.Count() })
                    .ToList();
                if (!itemCounts.Any())
                    continue;
                var max = itemCounts.OrderByDescending(x => x.Count).First();
                var min = itemCounts.OrderBy(x => x.Count).First();

                if (budgets.TryGetValue(max.Id, out var maxBi))
                {
                    result.Add(new UserBudgetItemCountDto
                    {
                        UserName = name,
                        EconomyCode = maxBi.EconomyCode,
                        FinancialCode = maxBi.FinancialCode,
                        Count = max.Count,
                        Type = "Max"
                    });
                }
                if (budgets.TryGetValue(min.Id, out var minBi))
                {
                    result.Add(new UserBudgetItemCountDto
                    {
                        UserName = name,
                        EconomyCode = minBi.EconomyCode,
                        FinancialCode = minBi.FinancialCode,
                        Count = min.Count,
                        Type = "Min"
                    });
                }
            }
            return result;
        }

        private static double SumCertificate(InspectionAcceptanceCertificate cert)
        {
            return cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
        }

        private static double SumCertificate(AdditionalInspectionAcceptanceCertificate cert)
        {
            return cert.SelectedProducts.Sum(p => p.UnitPrice * p.Quantity);
        }

        public async Task<IEnumerable<BudgetItemPaymentDto>> GetMostPaidBudgetItemsAsync(int top = 3)
        {
            var inspections = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();
            var entries = await _entryRepo.GetAllAsync();
            var budgets = await GetBudgetLookupAsync();

            var payments = new Dictionary<Guid, double>();

            foreach (var cert in inspections)
            {
                var entry = entries.FirstOrDefault(e => e.Id == cert.ProcurementEntryId);
                if (entry?.BudgetAllocationId == null) continue;
                var bid = entry.BudgetAllocationId.Value;
                payments[bid] = payments.GetValueOrDefault(bid) + SumCertificate(cert);
            }
            foreach (var cert in additionals)
            {
                var entry = entries.FirstOrDefault(e => e.Id == cert.ProcurementEntryId);
                if (entry?.BudgetAllocationId == null) continue;
                var bid = entry.BudgetAllocationId.Value;
                payments[bid] = payments.GetValueOrDefault(bid) + SumCertificate(cert);
            }

            var sorted = payments.OrderByDescending(k => k.Value).ToList();
            var topList = sorted.Take(top).ToList();
            var other = sorted.Skip(top).Sum(k => k.Value);

            var result = new List<BudgetItemPaymentDto>();
            foreach (var p in topList)
            {
                if (budgets.TryGetValue(p.Key, out var bi))
                {
                    result.Add(new BudgetItemPaymentDto
                    {
                        BudgetName = bi.Name,
                        EconomyCode = bi.EconomyCode,
                        FinancialCode = bi.FinancialCode,
                        TotalAmount = p.Value
                    });
                }
            }
            if (other > 0)
            {
                result.Add(new BudgetItemPaymentDto
                {
                    BudgetName = "Diğer",
                    TotalAmount = other
                });
            }
            return result;
        }

        public async Task<IEnumerable<BudgetItemPaymentDto>> GetLeastPaidBudgetItemsAsync(int top = 3)
        {
            var inspections = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();
            var entries = await _entryRepo.GetAllAsync();
            var budgets = await GetBudgetLookupAsync();

            var payments = new Dictionary<Guid, double>();

            foreach (var cert in inspections)
            {
                var entry = entries.FirstOrDefault(e => e.Id == cert.ProcurementEntryId);
                if (entry?.BudgetAllocationId == null) continue;
                var bid = entry.BudgetAllocationId.Value;
                payments[bid] = payments.GetValueOrDefault(bid) + SumCertificate(cert);
            }
            foreach (var cert in additionals)
            {
                var entry = entries.FirstOrDefault(e => e.Id == cert.ProcurementEntryId);
                if (entry?.BudgetAllocationId == null) continue;
                var bid = entry.BudgetAllocationId.Value;
                payments[bid] = payments.GetValueOrDefault(bid) + SumCertificate(cert);
            }

            var sorted = payments.OrderBy(k => k.Value).ToList();
            var topList = sorted.Take(top).ToList();
            var other = sorted.Skip(top).Sum(k => k.Value);

            var result = new List<BudgetItemPaymentDto>();
            foreach (var p in topList)
            {
                if (budgets.TryGetValue(p.Key, out var bi))
                {
                    result.Add(new BudgetItemPaymentDto
                    {
                        BudgetName = bi.Name,
                        EconomyCode = bi.EconomyCode,
                        FinancialCode = bi.FinancialCode,
                        TotalAmount = p.Value
                    });
                }
            }
            if (other > 0)
            {
                result.Add(new BudgetItemPaymentDto
                {
                    BudgetName = "Diğer",
                    TotalAmount = other
                });
            }
            return result;
        }

        public async Task<IEnumerable<BudgetItemOfferStatDto>> GetBudgetItemOfferTotalsAsync()
        {
            var entries = await _entryRepo.GetAllAsync();
            var offers = await _offerRepo.GetAllAsync();
            var budgets = await GetBudgetLookupAsync();

            var totals = new Dictionary<Guid, double>();

            foreach (var offer in offers)
            {
                var entry = entries.FirstOrDefault(e => e.Id == offer.ProcurementEntryId);
                if (entry?.BudgetAllocationId == null) continue;
                var bid = entry.BudgetAllocationId.Value;
                var totalPrice = offer.OfferItems.Sum(i => i.TotalAmount);
                totals[bid] = totals.GetValueOrDefault(bid) + totalPrice;
            }

            var result = new List<BudgetItemOfferStatDto>();
            foreach (var kvp in totals)
            {
                if (budgets.TryGetValue(kvp.Key, out var bi))
                {
                    result.Add(new BudgetItemOfferStatDto
                    {
                        BudgetName = bi.Name,
                        EconomyCode = bi.EconomyCode,
                        FinancialCode = bi.FinancialCode,
                        Value = kvp.Value
                    });
                }
            }
            return result;
        }

        public async Task<IEnumerable<BudgetItemOfferStatDto>> GetBudgetItemOfferAveragesAsync()
        {
            var entries = await _entryRepo.GetAllAsync();
            var offers = await _offerRepo.GetAllAsync();
            var budgets = await GetBudgetLookupAsync();

            var sums = new Dictionary<Guid, (double total, int count)>();

            foreach (var offer in offers)
            {
                var entry = entries.FirstOrDefault(e => e.Id == offer.ProcurementEntryId);
                if (entry?.BudgetAllocationId == null) continue;
                var bid = entry.BudgetAllocationId.Value;
                var totalPrice = offer.OfferItems.Sum(i => i.TotalAmount);
                if (sums.ContainsKey(bid))
                {
                    var tuple = sums[bid];
                    tuple.total += totalPrice;
                    tuple.count += 1;
                    sums[bid] = tuple;
                }
                else
                {
                    sums[bid] = (totalPrice, 1);
                }
            }

            var result = new List<BudgetItemOfferStatDto>();
            foreach (var kvp in sums)
            {
                if (budgets.TryGetValue(kvp.Key, out var bi))
                {
                    var avg = kvp.Value.count > 0 ? kvp.Value.total / kvp.Value.count : 0;
                    result.Add(new BudgetItemOfferStatDto
                    {
                        BudgetName = bi.Name,
                        EconomyCode = bi.EconomyCode,
                        FinancialCode = bi.FinancialCode,
                        Value = avg
                    });
                }
            }
            return result;
        }

        public async Task<IEnumerable<BudgetItemDto>> GetBudgetItemsWithoutEntriesAsync()
        {
            var entries = await _entryRepo.GetAllAsync();
            var used = entries.Where(e => e.BudgetAllocationId.HasValue)
                .Select(e => e.BudgetAllocationId!.Value)
                .ToHashSet();

            var allBudgets = await _budgetRepo.GetAllAsync();
            var result = allBudgets
                .Where(b => !used.Contains(b.Id))
                .Select(b => new BudgetItemDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    ItemCode = b.ItemCode,
                    CreatedByAdminId = b.CreatedByAdminId,
                    FinancialCode = b.FinancialCode,
                    EconomyCode = b.EconomyCode,
                    Records = b.Records?.Select(r => new BudgetRecordDto
                    {
                        Id = r.Id,
                        Date = r.Date,
                        Price = r.Price,
                        InvoiceNumber = r.InvoiceNumber,
                        WorkName = r.WorkName,
                        WorkReason = r.WorkReason
                    }).ToList()
                });
            return result;
        }
    }
}
