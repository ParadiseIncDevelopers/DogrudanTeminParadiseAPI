using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Helpers;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using System.Globalization;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ReportProductItemService : IReportProductItemService
    {
        private readonly MongoDBRepository<Product> _productRepo;
        private readonly MongoDBRepository<ProductItem> _itemRepo;
        private readonly MongoDBRepository<OfferLetter> _offerRepo;
        private readonly MongoDBRepository<InspectionAcceptanceCertificate> _inspectionRepo;
        private readonly MongoDBRepository<AdditionalInspectionAcceptanceCertificate> _addInspectionRepo;
        private readonly MongoDBRepository<Entreprise> _entrepriseRepo;

        public ReportProductItemService(
            MongoDBRepository<Product> productRepo,
            MongoDBRepository<ProductItem> itemRepo,
            MongoDBRepository<OfferLetter> offerRepo,
            MongoDBRepository<InspectionAcceptanceCertificate> inspectionRepo,
            MongoDBRepository<AdditionalInspectionAcceptanceCertificate> addInspectionRepo,
            MongoDBRepository<Entreprise> entrepriseRepo)
        {
            _productRepo = productRepo;
            _itemRepo = itemRepo;
            _offerRepo = offerRepo;
            _inspectionRepo = inspectionRepo;
            _addInspectionRepo = addInspectionRepo;
            _entrepriseRepo = entrepriseRepo;
        }

        public async Task<ProductItemTypeSpendingReportDto> GetSpendingByTypeAsync()
        {
            var products = await _productRepo.GetAllAsync();
            var items = await _itemRepo.GetAllAsync();
            var itemTypeLookup = items.ToDictionary(i => i.Id, i => i.Type);
            var nameToType = products
                .Where(p => itemTypeLookup.ContainsKey(p.ProductItemId))
                .ToDictionary(p => p.Name.ToLowerInvariant(), p => itemTypeLookup[p.ProductItemId]);

            var inspections = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var records = new List<(DateTime Date, ProductItemType Type, double Amount)>();

            foreach (var cert in inspections)
            {
                foreach (var sp in cert.SelectedProducts)
                {
                    if (nameToType.TryGetValue(sp.Name.ToLowerInvariant(), out var t))
                    {
                        records.Add((cert.InvoiceDate.Date, t, sp.UnitPrice * sp.Quantity));
                    }
                }
            }

            foreach (var cert in additionals)
            {
                foreach (var sp in cert.SelectedProducts)
                {
                    if (nameToType.TryGetValue(sp.Name.ToLowerInvariant(), out var t))
                    {
                        records.Add((cert.InvoiceDate.Date, t, sp.UnitPrice * sp.Quantity));
                    }
                }
            }

            if (records.Count == 0)
            {
                return new ProductItemTypeSpendingReportDto
                {
                    Weekly = [],
                    Monthly = [],
                    Quarterly = [],
                    Yearly = []
                };
            }

            var weekly = records
                .GroupBy(r =>
                {
                    var ci = CultureInfo.CurrentCulture;
                    var week = ci.Calendar.GetWeekOfYear(r.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return (r.Date.Year, Week: week);
                })
                .Select(g => new ProductItemTypeSpendingDto
                {
                    Period = FirstDateOfWeekISO8601(g.Key.Year, g.Key.Week).ToString("yyyy-MM-dd"),
                    ProductTotal = g.Where(x => x.Type == ProductItemType.PRODUCT).Sum(x => x.Amount),
                    ServiceTotal = g.Where(x => x.Type == ProductItemType.SERVICE).Sum(x => x.Amount)
                })
                .OrderBy(x => x.Period)
                .ToList();

            var monthly = records
                .GroupBy(r => new { r.Date.Year, r.Date.Month })
                .Select(g => new ProductItemTypeSpendingDto
                {
                    Period = $"{g.Key.Month:00}-{g.Key.Year}",
                    ProductTotal = g.Where(x => x.Type == ProductItemType.PRODUCT).Sum(x => x.Amount),
                    ServiceTotal = g.Where(x => x.Type == ProductItemType.SERVICE).Sum(x => x.Amount)
                })
                .OrderBy(x =>
                {
                    var parts = x.Period.Split('-');
                    return new DateTime(int.Parse(parts[1]), int.Parse(parts[0]), 1);
                })
                .ToList();

            var quarterly = records
                .GroupBy(r =>
                {
                    var quarter = (r.Date.Month - 1) / 3 + 1;
                    return new { r.Date.Year, Quarter = quarter };
                })
                .Select(g => new ProductItemTypeSpendingDto
                {
                    Period = $"Q{g.Key.Quarter}-{g.Key.Year}",
                    ProductTotal = g.Where(x => x.Type == ProductItemType.PRODUCT).Sum(x => x.Amount),
                    ServiceTotal = g.Where(x => x.Type == ProductItemType.SERVICE).Sum(x => x.Amount)
                })
                .OrderBy(x =>
                {
                    var parts = x.Period.Split('-', System.StringSplitOptions.RemoveEmptyEntries);
                    var q = int.Parse(parts[0].TrimStart('Q'));
                    var y = int.Parse(parts[1]);
                    return (y, q);
                })
                .ToList();

            var yearly = records
                .GroupBy(r => r.Date.Year)
                .Select(g => new ProductItemTypeSpendingDto
                {
                    Period = g.Key.ToString(),
                    ProductTotal = g.Where(x => x.Type == ProductItemType.PRODUCT).Sum(x => x.Amount),
                    ServiceTotal = g.Where(x => x.Type == ProductItemType.SERVICE).Sum(x => x.Amount)
                })
                .OrderBy(x => int.Parse(x.Period))
                .ToList();

            return new ProductItemTypeSpendingReportDto
            {
                Weekly = weekly,
                Monthly = monthly,
                Quarterly = quarterly,
                Yearly = yearly
            };
        }

        public async Task<IEnumerable<ProductItemCountDto>> GetMostUsedInProductsAsync(int top = 3)
        {
            var prods = await _productRepo.GetAllAsync();
            var items = (await _itemRepo.GetAllAsync()).ToDictionary(i => i.Id, i => i.Name);

            var counts = prods
                .GroupBy(p => p.ProductItemId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var result = new List<ProductItemCountDto>();
            foreach (var c in topList)
            {
                result.Add(new ProductItemCountDto
                {
                    ItemName = items.TryGetValue(c.Id, out var n) ? n : "Bilinmeyen",
                    Count = c.Count
                });
            }
            if (other > 0)
            {
                result.Add(new ProductItemCountDto { ItemName = "Diğer", Count = other });
            }
            return result;
        }

        public async Task<IEnumerable<ProductItemCountDto>> GetLeastUsedInProductsAsync(int top = 3)
        {
            var prods = await _productRepo.GetAllAsync();
            var items = (await _itemRepo.GetAllAsync()).ToDictionary(i => i.Id, i => i.Name);

            var counts = prods
                .GroupBy(p => p.ProductItemId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .OrderBy(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var result = new List<ProductItemCountDto>();
            foreach (var c in topList)
            {
                result.Add(new ProductItemCountDto
                {
                    ItemName = items.TryGetValue(c.Id, out var n) ? n : "Bilinmeyen",
                    Count = c.Count
                });
            }
            if (other > 0)
            {
                result.Add(new ProductItemCountDto { ItemName = "Diğer", Count = other });
            }
            return result;
        }

        private Dictionary<Guid, Guid> BuildNameToItemIdLookup(IEnumerable<Product> products)
        {
            return products.ToDictionary(p => Guid.Parse(p.Name), p => p.ProductItemId);
        }

        public async Task<IEnumerable<ProductItemCountDto>> GetMostUsedInOffersAsync(int top = 3)
        {
            var offers = await _offerRepo.GetAllAsync();
            var products = await _productRepo.GetAllAsync();
            var nameToItem = BuildNameToItemIdLookup(products);
            var itemNames = (await _itemRepo.GetAllAsync()).ToDictionary(i => i.Id, i => i.Name);

            var counts = new Dictionary<Guid, int>();
            foreach (var offer in offers)
            {
                foreach (var item in offer.OfferItems)
                {
                    if (nameToItem.TryGetValue(Guid.Parse(item.Name), out var pid))
                    {
                        counts[pid] = counts.GetValueOrDefault(pid) + 1;
                    }
                }
            }

            var sorted = counts.OrderByDescending(k => k.Value).ToList();
            var topList = sorted.Take(top).ToList();
            var other = sorted.Skip(top).Sum(k => k.Value);
            var result = new List<ProductItemCountDto>();
            foreach (var kv in topList)
            {
                result.Add(new ProductItemCountDto
                {
                    ItemName = itemNames.TryGetValue(kv.Key, out var n) ? n : "Bilinmeyen",
                    Count = kv.Value
                });
            }
            if (other > 0)
            {
                result.Add(new ProductItemCountDto { ItemName = "Diğer", Count = other });
            }
            return result;
        }

        public async Task<IEnumerable<ProductItemCountDto>> GetLeastUsedInOffersAsync(int top = 3)
        {
            var offers = await _offerRepo.GetAllAsync();
            var products = await _productRepo.GetAllAsync();
            var nameToItem = BuildNameToItemIdLookup(products);
            var itemNames = (await _itemRepo.GetAllAsync()).ToDictionary(i => i.Id, i => i.Name);

            var counts = new Dictionary<Guid, int>();
            foreach (var offer in offers)
            {
                foreach (var item in offer.OfferItems)
                {
                    if (nameToItem.TryGetValue(Guid.Parse(item.Name), out var pid))
                    {
                        counts[pid] = counts.GetValueOrDefault(pid) + 1;
                    }
                }
            }

            var sorted = counts.OrderBy(k => k.Value).ToList();
            var topList = sorted.Take(top).ToList();
            var other = sorted.Skip(top).Sum(k => k.Value);
            var result = new List<ProductItemCountDto>();
            foreach (var kv in topList)
            {
                result.Add(new ProductItemCountDto
                {
                    ItemName = itemNames.TryGetValue(kv.Key, out var n) ? n : "Bilinmeyen",
                    Count = kv.Value
                });
            }
            if (other > 0)
            {
                result.Add(new ProductItemCountDto { ItemName = "Diğer", Count = other });
            }
            return result;
        }

        public async Task<IEnumerable<FirmProductItemCountDto>> GetFirmOfferExtremesAsync(Guid firmId)
        {
            var offers = (await _offerRepo.GetAllAsync()).Where(o => o.EntrepriseId == firmId).ToList();
            if (!offers.Any())
                return new List<FirmProductItemCountDto>();

            var products = await _productRepo.GetAllAsync();
            var nameToItem = BuildNameToItemIdLookup(products);
            var itemNames = (await _itemRepo.GetAllAsync()).ToDictionary(i => i.Id, i => i.Name);

            var counts = new Dictionary<Guid, int>();
            foreach (var offer in offers)
            {
                foreach (var item in offer.OfferItems)
                {
                    if (nameToItem.TryGetValue(Guid.Parse(item.Name), out var pid))
                    {
                        counts[pid] = counts.GetValueOrDefault(pid) + 1;
                    }
                }
            }
            if (counts.Count == 0)
                return new List<FirmProductItemCountDto>();

            var max = counts.OrderByDescending(x => x.Value).First();
            var min = counts.OrderBy(x => x.Value).First();
            var firmName = (await _entrepriseRepo.GetByIdAsync(firmId))?.Unvan ?? "Bilinmeyen";

            return new List<FirmProductItemCountDto>
            {
                new FirmProductItemCountDto
                {
                    FirmName = firmName,
                    ItemName = itemNames.TryGetValue(max.Key, out var mx) ? mx : "Bilinmeyen",
                    Count = max.Value,
                    Type = "Max"
                },
                new FirmProductItemCountDto
                {
                    FirmName = firmName,
                    ItemName = itemNames.TryGetValue(min.Key, out var mn) ? mn : "Bilinmeyen",
                    Count = min.Value,
                    Type = "Min"
                }
            };
        }

        private static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            var jan4 = new DateTime(year, 1, 4);
            var weekday = (int)jan4.DayOfWeek;
            if (weekday == 0) weekday = 7;
            var start = jan4.AddDays(1 - weekday);
            return start.AddDays((weekOfYear - 1) * 7);
        }
    }
}
