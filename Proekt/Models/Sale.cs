using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt.Models;

public class Sale
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountedAmount { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
