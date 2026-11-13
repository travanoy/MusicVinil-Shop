using Proekt.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt.Models;

public class PromotionCondition
{
    public int Id { get; set; }

    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; } = null!;

    public ConditionField Field { get; set; }
    public ConditionOperator Operator { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? ValueMax { get; set; }
}
