using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt.Models;

public class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public int RecordId { get; set; }
    public Record Record { get; set; } = null!;

    public decimal UnitPrice { get; set; }
    public decimal PromotionDiscount { get; set; }
    public string? AppliedPromotionName { get; set; }
    public decimal FinalPrice { get; set; }
}
