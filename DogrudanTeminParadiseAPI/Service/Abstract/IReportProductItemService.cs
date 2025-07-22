using DogrudanTeminParadiseAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportProductItemService
    {
        Task<ProductItemTypeSpendingReportDto> GetSpendingByTypeAsync();
        Task<IEnumerable<ProductItemCountDto>> GetMostUsedInProductsAsync(int top = 3);
        Task<IEnumerable<ProductItemCountDto>> GetLeastUsedInProductsAsync(int top = 3);
        Task<IEnumerable<ProductItemCountDto>> GetMostUsedInOffersAsync(int top = 3);
        Task<IEnumerable<ProductItemCountDto>> GetLeastUsedInOffersAsync(int top = 3);
        Task<IEnumerable<FirmProductItemCountDto>> GetFirmOfferExtremesAsync(Guid firmId);
    }
}
