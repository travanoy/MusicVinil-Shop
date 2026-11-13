using Proekt.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt.Models;

public class Record
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int TrackCount { get; set; }
    public Genre Genre { get; set; }
    public int ReleaseYear { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public RecordStatus Status { get; set; } = RecordStatus.InStock;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    public int? ReservedForCustomerId { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
