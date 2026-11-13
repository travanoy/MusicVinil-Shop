namespace Proekt.Repositories.repos;

using Microsoft.EntityFrameworkCore;
using Proekt.Data;
using Proekt.Models;
using Proekt.Repositories.Interfaces;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly MusicStoreDbContext _db;
    public EmployeeRepository(MusicStoreDbContext db) => _db = db;

    public async Task<Employee?> GetByIdAsync(int id)
        => await _db.Employees.FindAsync(id);

    public async Task<Employee?> GetByLoginAsync(string login)
        => await _db.Employees.FirstOrDefaultAsync(e => e.Login == login);

    public async Task<List<Employee>> GetAllAsync()
        => await _db.Employees.OrderBy(e => e.LastName).ToListAsync();

    public async Task<Employee?> GetFirstAsync()
        => await _db.Employees.FirstOrDefaultAsync();

    public async Task<bool> ExistsAsync(string login)
        => await _db.Employees.AnyAsync(e => e.Login == login);

    public async Task AddAsync(Employee employee)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp != null)
        {
            _db.Employees.Remove(emp);
            await _db.SaveChangesAsync();
        }
    }
}