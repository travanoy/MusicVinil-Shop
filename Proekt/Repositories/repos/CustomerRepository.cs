namespace Proekt.Repositories.repos;

using Microsoft.EntityFrameworkCore;
using Proekt.Data;
using Proekt.Models;
using Proekt.Repositories.Interfaces;

public class CustomerRepository : ICustomerRepository
{
    private readonly MusicStoreDbContext _db;
    public CustomerRepository(MusicStoreDbContext db) => _db = db;

    public async Task<Customer?> GetByIdAsync(int id)
        => await _db.Customers.FindAsync(id);

    public async Task<Customer?> GetByLoginAsync(string login)
        => await _db.Customers.FirstOrDefaultAsync(c => c.Login == login);

    public async Task<List<Customer>> GetAllAsync()
        => await _db.Customers.OrderBy(c => c.LastName).ToListAsync();

    public async Task<bool> ExistsAsync(string login)
        => await _db.Customers.AnyAsync(c => c.Login == login);

    public async Task AddAsync(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }
}