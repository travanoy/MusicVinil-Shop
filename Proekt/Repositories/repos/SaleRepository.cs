namespace Proekt.Repositories.repos;

using Microsoft.EntityFrameworkCore;
using Proekt.Data;
using Proekt.Models;
using Proekt.Repositories.Interfaces;

public class SaleRepository : ISaleRepository
{
    private readonly MusicStoreDbContext _db;
    public SaleRepository(MusicStoreDbContext db) => _db = db;

    public async Task AddAsync(Sale sale)
    {
        _db.Sales.Add(sale);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Sale>> GetByCustomerAsync(int customerId)
        => await _db.Sales
            .Include(s => s.Items).ThenInclude(si => si.Record)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

    public async Task<List<SaleItem>> GetSaleItemsInPeriodAsync(DateTime from, DateTime to)
        => await _db.SaleItems
            .Include(si => si.Record)
            .Include(si => si.Sale)
            .Where(si => si.Sale.SaleDate >= from && si.Sale.SaleDate <= to)
            .ToListAsync();
}