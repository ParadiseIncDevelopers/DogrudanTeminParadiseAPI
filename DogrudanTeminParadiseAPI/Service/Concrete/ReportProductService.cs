using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;
using System.Globalization;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ReportProductService : IReportProductService
    {
        private readonly MongoDBRepository<ProcurementEntryEditor> _editorRepo;
        private readonly MongoDBRepository<InspectionAcceptanceCertificate> _inspectionRepo;
        private readonly MongoDBRepository<AdditionalInspectionAcceptanceCertificate> _addInspectionRepo;
        private readonly MongoDBRepository<OfferLetter> _offerRepo;
        private readonly MongoDBRepository<Entreprise> _entrepriseRepo;

        public ReportProductService(
            MongoDBRepository<ProcurementEntryEditor> editorRepo,
            MongoDBRepository<InspectionAcceptanceCertificate> inspectionRepo,
            MongoDBRepository<AdditionalInspectionAcceptanceCertificate> addInspectionRepo,
            MongoDBRepository<OfferLetter> offerRepo,
            MongoDBRepository<Entreprise> entrepriseRepo)
        {
            _editorRepo = editorRepo;
            _inspectionRepo = inspectionRepo;
            _addInspectionRepo = addInspectionRepo;
            _offerRepo = offerRepo;
            _entrepriseRepo = entrepriseRepo;
        }

        public async Task<IEnumerable<ProductCountDto>> GetMostUsedProductsAsync(int top = 3)
        {
            var editors = await _editorRepo.GetAllAsync();
            var counts = editors
                .SelectMany(e => e.OfferItems)
                .GroupBy(i => i.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var result = topList
                .Select(x => new ProductCountDto { ProductName = x.Name, Count = x.Count })
                .ToList();
            if (other > 0)
                result.Add(new ProductCountDto { ProductName = "Diğer", Count = other });
            return result;
        }

        public async Task<IEnumerable<ProductCountDto>> GetLeastUsedProductsAsync(int top = 3)
        {
            var editors = await _editorRepo.GetAllAsync();
            var counts = editors
                .SelectMany(e => e.OfferItems)
                .GroupBy(i => i.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderBy(x => x.Count)
                .ToList();

            var topList = counts.Take(top).ToList();
            var other = counts.Skip(top).Sum(x => x.Count);
            var result = topList
                .Select(x => new ProductCountDto { ProductName = x.Name, Count = x.Count })
                .ToList();
            if (other > 0)
                result.Add(new ProductCountDto { ProductName = "Diğer", Count = other });
            return result;
        }

        public async Task<IEnumerable<ProductInspectionStatDto>> GetMostInspectedProductsAsync(int top = 5)
        {
            var normals = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var allItems = normals
                .SelectMany(c => c.SelectedProducts.Select(p => new { p.Name, p.Quantity }))
                .Concat(additionals.SelectMany(c => c.SelectedProducts.Select(p => new { p.Name, p.Quantity })));

            var grouped = allItems
                .GroupBy(i => i.Name)
                .Select(g => new ProductInspectionStatDto
                {
                    ProductName = g.Key,
                    CertificateCount = g.Count(),
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.CertificateCount)
                .Take(top)
                .ToList();

            return grouped;
        }

        public async Task<IEnumerable<ProductInspectionStatDto>> GetLeastInspectedProductsAsync(int top = 5)
        {
            var normals = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var allItems = normals
                .SelectMany(c => c.SelectedProducts.Select(p => new { p.Name, p.Quantity }))
                .Concat(additionals.SelectMany(c => c.SelectedProducts.Select(p => new { p.Name, p.Quantity })));

            var grouped = allItems
                .GroupBy(i => i.Name)
                .Select(g => new ProductInspectionStatDto
                {
                    ProductName = g.Key,
                    CertificateCount = g.Count(),
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderBy(x => x.CertificateCount)
                .Take(top)
                .ToList();

            return grouped;
        }

        public async Task<IEnumerable<ProductOfferDto>> GetHighestOffersAsync(string productName, int top = 3)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<ProductOfferDto>();

            var offers = await _offerRepo.GetAllAsync();
            var firms = (await _entrepriseRepo.GetAllAsync()).ToDictionary(e => e.Id, e => e.Unvan);

            var items = offers
                .SelectMany(o => o.OfferItems.Select(i => new { Offer = o, Item = i }))
                .Where(x => x.Item.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Item.UnitPrice)
                .Take(top)
                .Select(x => new ProductOfferDto
                {
                    FirmName = firms.TryGetValue(x.Offer.EntrepriseId, out var name) ? name : "Bilinmeyen",
                    UnitPrice = x.Item.UnitPrice,
                    TotalAmount = x.Item.TotalAmount
                })
                .ToList();

            return items;
        }

        public async Task<IEnumerable<ProductOfferDto>> GetLowestOffersAsync(string productName, int top = 3)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<ProductOfferDto>();

            var offers = await _offerRepo.GetAllAsync();
            var firms = (await _entrepriseRepo.GetAllAsync()).ToDictionary(e => e.Id, e => e.Unvan);

            var items = offers
                .SelectMany(o => o.OfferItems.Select(i => new { Offer = o, Item = i }))
                .Where(x => x.Item.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Item.UnitPrice)
                .Take(top)
                .Select(x => new ProductOfferDto
                {
                    FirmName = firms.TryGetValue(x.Offer.EntrepriseId, out var name) ? name : "Bilinmeyen",
                    UnitPrice = x.Item.UnitPrice,
                    TotalAmount = x.Item.TotalAmount
                })
                .ToList();

            return items;
        }

        public async Task<ProductQuantityReportDto> GetPurchaseQuantityReportAsync()
        {
            var normals = await _inspectionRepo.GetAllAsync();
            var additionals = await _addInspectionRepo.GetAllAsync();

            var records = normals
                .SelectMany(c => c.SelectedProducts.Select(p => new { Date = c.InvoiceDate.Date, p.Name, p.Quantity }))
                .Concat(additionals.SelectMany(c => c.SelectedProducts.Select(p => new { Date = c.InvoiceDate.Date, p.Name, p.Quantity })))
                .ToList();

            if (records.Count == 0)
            {
                return new ProductQuantityReportDto
                {
                    Weekly = new List<ProductQuantityExtremeDto>(),
                    Monthly = new List<ProductQuantityExtremeDto>(),
                    Quarterly = new List<ProductQuantityExtremeDto>(),
                    Yearly = new List<ProductQuantityExtremeDto>()
                };
            }

            var weekly = records
                .GroupBy(r =>
                {
                    var ci = CultureInfo.CurrentCulture;
                    var week = ci.Calendar.GetWeekOfYear(r.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return (r.Date.Year, Week: week);
                })
                .Select(g =>
                {
                    var firstDay = FirstDateOfWeekISO8601(g.Key.Year, g.Key.Week).ToString("yyyy-MM-dd");
                    var prodGroups = g.GroupBy(x => x.Name).Select(pg => new { Name = pg.Key, Qty = pg.Sum(x => x.Quantity) }).ToList();
                    var max = prodGroups.OrderByDescending(p => p.Qty).First();
                    var min = prodGroups.OrderBy(p => p.Qty).First();
                    return new ProductQuantityExtremeDto
                    {
                        Period = firstDay,
                        MaxProduct = max.Name,
                        MaxQuantity = max.Qty,
                        MinProduct = min.Name,
                        MinQuantity = min.Qty
                    };
                })
                .OrderBy(x => x.Period)
                .ToList();

            var monthly = records
                .GroupBy(r => new { r.Date.Year, r.Date.Month })
                .Select(g =>
                {
                    var label = $"{g.Key.Month:00}-{g.Key.Year}";
                    var prodGroups = g.GroupBy(x => x.Name).Select(pg => new { Name = pg.Key, Qty = pg.Sum(x => x.Quantity) }).ToList();
                    var max = prodGroups.OrderByDescending(p => p.Qty).First();
                    var min = prodGroups.OrderBy(p => p.Qty).First();
                    return new ProductQuantityExtremeDto
                    {
                        Period = label,
                        MaxProduct = max.Name,
                        MaxQuantity = max.Qty,
                        MinProduct = min.Name,
                        MinQuantity = min.Qty
                    };
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
                .Select(g =>
                {
                    var label = $"Q{g.Key.Quarter}-{g.Key.Year}";
                    var prodGroups = g.GroupBy(x => x.Name).Select(pg => new { Name = pg.Key, Qty = pg.Sum(x => x.Quantity) }).ToList();
                    var max = prodGroups.OrderByDescending(p => p.Qty).First();
                    var min = prodGroups.OrderBy(p => p.Qty).First();
                    return new ProductQuantityExtremeDto
                    {
                        Period = label,
                        MaxProduct = max.Name,
                        MaxQuantity = max.Qty,
                        MinProduct = min.Name,
                        MinQuantity = min.Qty
                    };
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
                .Select(g =>
                {
                    var label = g.Key.ToString();
                    var prodGroups = g.GroupBy(x => x.Name).Select(pg => new { Name = pg.Key, Qty = pg.Sum(x => x.Quantity) }).ToList();
                    var max = prodGroups.OrderByDescending(p => p.Qty).First();
                    var min = prodGroups.OrderBy(p => p.Qty).First();
                    return new ProductQuantityExtremeDto
                    {
                        Period = label,
                        MaxProduct = max.Name,
                        MaxQuantity = max.Qty,
                        MinProduct = min.Name,
                        MinQuantity = min.Qty
                    };
                })
                .OrderBy(x => int.Parse(x.Period))
                .ToList();

            return new ProductQuantityReportDto
            {
                Weekly = weekly,
                Monthly = monthly,
                Quarterly = quarterly,
                Yearly = yearly
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
