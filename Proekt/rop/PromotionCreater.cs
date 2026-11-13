using Proekt.Enums;
using Proekt.Models;

namespace Proekt.rop;



public static class PromotionCreater
{
    public static (string name, decimal percent)? GetBestDiscount(
        Record record, List<Promotion> activePromotions)
    {
        string? bestName = null;
        decimal bestPercent = 0;

        foreach (var promo in activePromotions)
        {
            if (!promo.IsActive || !promo.Conditions.Any()) continue;

            bool allMatch = promo.Conditions.All(c => Check(record, c));

            if (allMatch && promo.DiscountPercent > bestPercent)
            {
                bestName = promo.Name;
                bestPercent = promo.DiscountPercent;
            }
        }

        return bestName != null ? (bestName, bestPercent) : null;
    }

    private static bool Check(Record record, PromotionCondition c)
    {
        string val = c.Field switch
        {
            ConditionField.Genre => record.Genre.ToString(),
            ConditionField.Artist => record.Artist,
            ConditionField.Publisher => record.Publisher,
            ConditionField.Title => record.Title,
            ConditionField.ReleaseYear => record.ReleaseYear.ToString(),
            ConditionField.SellingPrice => record.SellingPrice.ToString(),
            ConditionField.TrackCount => record.TrackCount.ToString(),
            _ => ""
        };

        if (c.Field is ConditionField.Genre or ConditionField.Artist
            or ConditionField.Publisher or ConditionField.Title)
        {
            return c.Operator switch
            {
                ConditionOperator.Equals => val.Equals(c.Value, StringComparison.OrdinalIgnoreCase),
                ConditionOperator.NotEquals => !val.Equals(c.Value, StringComparison.OrdinalIgnoreCase),
                ConditionOperator.Contains => val.Contains(c.Value, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        if (!decimal.TryParse(val, out var f)) return false;
        if (!decimal.TryParse(c.Value, out var v)) return false;

        return c.Operator switch
        {
            ConditionOperator.Equals => f == v,
            ConditionOperator.NotEquals => f != v,
            ConditionOperator.GreaterThan => f > v,
            ConditionOperator.GreaterOrEqual => f >= v,
            ConditionOperator.LessThan => f < v,
            ConditionOperator.LessOrEqual => f <= v,
            ConditionOperator.Between => decimal.TryParse(c.ValueMax, out var max) && f >= v && f <= max,
            _ => false
        };
    }
}