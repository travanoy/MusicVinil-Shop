namespace Proekt.Repositories.Interfaces;

using Proekt.Models;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByLoginAsync(string login);
    Task<List<Customer>> GetAllAsync();
    Task<bool> ExistsAsync(string login);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
}