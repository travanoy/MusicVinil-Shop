namespace Proekt.Repositories.Interfaces;


using Proekt.Models;
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByLoginAsync(string login);
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetFirstAsync();
    Task<bool> ExistsAsync(string login);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
}