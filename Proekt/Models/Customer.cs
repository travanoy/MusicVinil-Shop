using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt.Models;

public class Customer
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
  
    public decimal TotalSpent { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public ICollection<Sale> Purchases { get; set; } = new List<Sale>();
    public ICollection<Record> ReservedRecords { get; set; } = new List<Record>();

    public decimal GetDiscountPercent() => TotalSpent switch
    {
        >= 50_000m => 15m,
        >= 20_000m => 10m,
        >= 10_000m => 7m,
        >= 5_000m => 5m,
        >= 1_000m => 3m,
        _ => 0m
    };
}
