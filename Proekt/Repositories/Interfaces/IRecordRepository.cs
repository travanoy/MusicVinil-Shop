 namespace Proekt.Repositories.Interfaces;

using Proekt.Enums;
using Proekt.Models;
public interface IRecordRepository
{
    Task<Record?> GetByIdAsync(int id);
    Task<List<Record>> GetAllAsync();
    Task<List<Record>> GetInStockAsync();
    Task<List<Record>> GetByIdsAsync(List<int> ids);
    Task<List<Record>> SearchAsync(string? title, string? artist, Genre? genre);
    Task<List<Record>> GetNewArrivalsAsync(int count);
    Task<List<Record>> GetReservedByCustomerAsync(int customerId);
    Task AddAsync(Record record);
    Task UpdateAsync(Record record);
    Task DeleteAsync(int id);
    Task SaveAsync();
}