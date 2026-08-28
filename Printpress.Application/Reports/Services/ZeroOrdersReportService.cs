using Printpress.Domain;

namespace Printpress.Application;

internal sealed class ZeroOrdersReportService(IUnitOfWork unitOfWork) : IZeroOrdersReportService
{
    public async Task<ZeroOrdersReportDto> GetReportAsync(DateTime? dateFrom, DateTime? dateToExclusive)
    {
        if (dateFrom is not null && dateToExclusive is not null && dateFrom >= dateToExclusive)
            throw new ValidationExeption("تاريخ البداية يجب أن يكون قبل تاريخ النهاية أو مساوياً له");

        var orders = (await unitOfWork.OrderRepository.FilterAsync(
                o => o.IsZeroOrder
                    && (dateFrom == null || o.CreatedAt >= dateFrom)
                    && (dateToExclusive == null || o.CreatedAt < dateToExclusive),
                nameof(Order.Client),
                nameof(Order.Services),
                $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}"))
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        var rows = orders.Select(o => new ZeroOrderReportRowDto
        {
            OrderId = o.Id,
            OrderName = o.Name,
            ClientName = o.Client?.Name ?? "—",
            CreatedAt = o.CreatedAt,
            Status = o.Status.ToString(),
            ServiceCount = (o.Services ?? []).Count(s => !s.IsDeleted),
            ItemCount = (o.OrderGroups ?? []).Where(g => !g.IsDeleted)
                .SelectMany(g => g.Items ?? [])
                .Count(i => !i.IsDeleted),
            TotalPrice = o.TotalPrice ?? 0
        }).ToList();

        return new ZeroOrdersReportDto
        {
            Orders = rows,
            OrderCount = rows.Count,
            ItemCount = rows.Sum(r => r.ItemCount)
        };
    }
}
