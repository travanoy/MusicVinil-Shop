namespace Proekt.Repositories.repos;

using Microsoft.EntityFrameworkCore;
using Proekt.Data;
using Proekt.Enums;
using Proekt.Models;
using Proekt.Repositories.Interfaces;

public class RecordRepository : IRecordRepository
{
    private readonly MusicStoreDbContext _db;
    public RecordRepository(MusicStoreDbContext db) => _db = db;

    public async Task<Record?> GetByIdAsync(int id)
        => await _db.Records.FindAsync(id);

    public async Task<List<Record>> GetAllAsync()
        => await _db.Records.OrderBy(r => r.Title).ToListAsync();

    public async Task<List<Record>> GetInStockAsync()
        => await _db.Records
            .Where(r => r.Status == RecordStatus.InStock)
            .OrderBy(r => r.Title).ToListAsync();

    public async Task<List<Record>> GetByIdsAsync(List<int> ids)
        => await _db.Records
            .Where(r => ids.Contains(r.Id) && r.Status == RecordStatus.InStock)
            .ToListAsync();

    public async Task<List<Record>> SearchAsync(string? title, string? artist, Genre? genre)
    {
        var q = _db.Records.Where(r => r.Status == RecordStatus.InStock).AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            q = q.Where(r => r.Title.Contains(title));
        if (!string.IsNullOrWhiteSpace(artist))
            q = q.Where(r => r.Artist.Contains(artist));
        if (genre.HasValue && genre != default)
            q = q.Where(r => r.Genre == genre);

        return await q.OrderBy(r => r.Title).ToListAsync();
    }

    public async Task<List<Record>> GetNewArrivalsAsync(int count)
        => await _db.Records
            .Where(r => r.Status == RecordStatus.InStock)
            .OrderByDescending(r => r.DateAdded)
            .Take(count).ToListAsync();

    public async Task<List<Record>> GetReservedByCustomerAsync(int customerId)
        => await _db.Records
            .Where(r => r.ReservedForCustomerId == customerId
                     && r.Status == RecordStatus.Reserved)
            .ToListAsync();

    public async Task AddAsync(Record record)
    {
        _db.Records.Add(record);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Record record)
    {
        _db.Records.Update(record);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rec = await _db.Records.FindAsync(id);
        if (rec != null)
        {
            _db.Records.Remove(rec);
            await _db.SaveChangesAsync();
        }
    }

    public async Task SaveAsync()
        => await _db.SaveChangesAsync();
}