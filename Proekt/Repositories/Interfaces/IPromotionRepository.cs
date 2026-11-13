namespace Proekt.Repositories.Interfaces;
using Proekt.Models;

public interface IPromotionRepository
{
    Task<List<Promotion>> GetAllAsync();
    Task<List<Promotion>> GetActiveAsync();
    Task AddAsync(Promotion promotion);
    Task DeleteAsync(int id);
}