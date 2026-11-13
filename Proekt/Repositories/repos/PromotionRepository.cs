namespace Proekt.Repositories.repos;

using Microsoft.EntityFrameworkCore;
using Proekt.Data;
using Proekt.Models;
using Proekt.Repositories.Interfaces;

public class PromotionRepository : IPromotionRepository
{
    private readonly MusicStoreDbContext _db;
    public PromotionRepository(MusicStoreDbContext db) => _db = db;

    public async Task<List<Promotion>> GetAllAsync()
        => await _db.Promotions
            .Include(p => p.Conditions)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();

    public async Task<List<Promotion>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _db.Promotions
            .Include(p => p.Conditions)
            .Where(p => p.StartDate <= now && p.EndDate >= now)
            .ToListAsync();
    }

    public async Task AddAsync(Promotion promotion)
    {
        _db.Promotions.Add(promotion);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var promo = await _db.Promotions.FindAsync(id);
        if (promo != null)
        {
            _db.Promotions.Remove(promo);
            await _db.SaveChangesAsync();
        }
    }
}