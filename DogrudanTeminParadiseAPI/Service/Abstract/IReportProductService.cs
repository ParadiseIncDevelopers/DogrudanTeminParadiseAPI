using DogrudanTeminParadiseAPI.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportProductService
    {
        Task<IEnumerable<ProductCountDto>> GetMostUsedProductsAsync(int top = 3);
        Task<IEnumerable<ProductCountDto>> GetLeastUsedProductsAsync(int top = 3);
        Task<IEnumerable<ProductInspectionStatDto>> GetMostInspectedProductsAsync(int top = 5);
        Task<IEnumerable<ProductInspectionStatDto>> GetLeastInspectedProductsAsync(int top = 5);
        Task<IEnumerable<ProductOfferDto>> GetHighestOffersAsync(string productName, int top = 3);
        Task<IEnumerable<ProductOfferDto>> GetLowestOffersAsync(string productName, int top = 3);
        Task<ProductQuantityReportDto> GetPurchaseQuantityReportAsync();
    }
}
