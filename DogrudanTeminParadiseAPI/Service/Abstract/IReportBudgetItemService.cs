using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IReportBudgetItemService
    {
        Task<IEnumerable<BudgetItemCountDto>> GetMostEntryBudgetItemsAsync(int top = 3);
        Task<IEnumerable<BudgetItemCountDto>> GetLeastEntryBudgetItemsAsync(int top = 3);
        Task<IEnumerable<UserBudgetItemCountDto>> GetUserEntryExtremesAsync();
        Task<IEnumerable<BudgetItemPaymentDto>> GetMostPaidBudgetItemsAsync(int top = 3);
        Task<IEnumerable<BudgetItemPaymentDto>> GetLeastPaidBudgetItemsAsync(int top = 3);
        Task<IEnumerable<BudgetItemOfferStatDto>> GetBudgetItemOfferTotalsAsync();
        Task<IEnumerable<BudgetItemOfferStatDto>> GetBudgetItemOfferAveragesAsync();
        Task<IEnumerable<BudgetItemDto>> GetBudgetItemsWithoutEntriesAsync();
    }
}
