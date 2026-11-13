namespace Proekt.Repositories.Interfaces;

using Proekt.Models;

public interface ISaleRepository
{
    Task AddAsync(Sale sale);
    Task<List<Sale>> GetByCustomerAsync(int customerId);
    Task<List<SaleItem>> GetSaleItemsInPeriodAsync(DateTime from, DateTime to);
}