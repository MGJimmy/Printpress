namespace Printpress.Application;

internal static class OrderInventoryItemsCalculator
{
    public static int CalculateUnitsPerCarton(int? packsPerCarton, int? unitsPerPack)
    {
        if (packsPerCarton is null or 0 || unitsPerPack is null or 0)
            return 1;
        return packsPerCarton.Value * unitsPerPack.Value;
    }

    public static decimal CalculateCartonsFromUnits(int units, int unitsPerCarton)
    {
        if (unitsPerCarton <= 0) return units;
        return Math.Round((decimal)units / unitsPerCarton, 2);
    }

    public static decimal CalculatePaperUsed(List<OrderItemUsageProjection> items)
    {
        return items.Sum(item => CalculatePaperUsedForItem(item));
    }

    private static decimal CalculatePaperUsedForItem(OrderItemUsageProjection item)
    {
        if (item.NumberOfPrintingFaces <= 0) return 0;
        return Math.Round((decimal)(item.Quantity * item.NumberOfPages) / item.NumberOfPrintingFaces, 2);
    }

    public static decimal CalculateExpectedWaste(decimal paperUsed, int wastePercent)
    {
        return Math.Round(paperUsed * wastePercent / 100m, 2);
    }

    public static decimal CalculateDifference(int unitsOut, decimal paperUsed, decimal expectedWaste)
    {
        return Math.Round(unitsOut - (paperUsed + expectedWaste), 2);
    }
}
