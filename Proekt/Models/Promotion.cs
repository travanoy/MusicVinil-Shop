using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt.Models;

public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

    public ICollection<PromotionCondition> Conditions { get; set; } = new List<PromotionCondition>();
}
