using System.Collections.Specialized;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Printpress.Domain;
using QuestPDF.Infrastructure;

namespace Printpress.Application
{
    internal class InvoiceReportFactory(IUnitOfWork unitOfWork, IConfiguration configuration) : IReportFactory
    {
        public string ReportName => "invoice";
        public async Task<IDocument> GenerateReport(NameValueCollection queryParams)
        {
            Guid id = Guid.Parse(queryParams.GetValues("id").FirstOrDefault());

            QuestPDF.Settings.License = LicenseType.Community;

            string[] includes = [
                     nameof(Order.Client),
                     nameof(Order.Services),
                     $"{nameof(Order.Services)}.{nameof(OrderService.Service)}",
                     $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}.{nameof(OrderItem.Details)}",
                     $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}.{nameof(OrderGroupService.Service)}",
                     $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}.{nameof(OrderGroupService.Service)}.{nameof(Service.ServiceCategory)}",
                     nameof(Order.SellingItems),
                     $"{nameof(Order.SellingItems)}.{nameof(OrderSellingItem.InventoryItem)}"
            ];

            var model = await unitOfWork.OrderRepository.FirstOrDefaultAsync(order => order.Id == id, true, includes);

            var isPartial = ApplyGroupFilter(model, queryParams.Get("groupIds"));
            // Preserve legacy links while allowing the dialog to choose selling items independently.
            var includeSellingItems = bool.TryParse(queryParams.Get("includeSellingItems"), out var include)
                ? include
                : !isPartial;
            var excludesSellingItems = !includeSellingItems && (model.SellingItems?.Count ?? 0) > 0;
            if (!includeSellingItems)
                model.SellingItems = [];

            isPartial |= excludesSellingItems;
            if (isPartial)
            {
                model.TotalPrice = (model.OrderGroups ?? [])
                    .SelectMany(g => g.Items ?? [])
                    .Where(i => !i.IsDeleted)
                    .Sum(i => i.Price * i.Quantity)
                    + (model.SellingItems ?? []).Sum(i => i.Price * (decimal)i.Quantity);
            }

            var document = new InvoiceReport(model, configuration, isPartial);

            return document;

        }

        private static bool ApplyGroupFilter(Order model, string groupIdsRaw)
        {
            if (model is null || groupIdsRaw is null)
                return false;

            var selectedGroupIds = groupIdsRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => Guid.TryParse(value, out var id) ? id : (Guid?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToHashSet();

            if (selectedGroupIds.Count == 0 && !string.IsNullOrWhiteSpace(groupIdsRaw))
                return false;

            var originalGroupCount = model.OrderGroups?.Count ?? 0;
            model.OrderGroups = (model.OrderGroups ?? [])
                .Where(g => selectedGroupIds.Contains(g.Id))
                .ToList();

            var selectedServiceIds = model.OrderGroups
                .SelectMany(g => g.OrderGroupServices ?? [])
                .Select(gs => gs.ServiceId)
                .ToHashSet();

            model.Services = (model.Services ?? [])
                .Where(s => selectedServiceIds.Contains(s.ServiceId))
                .ToList();

            return model.OrderGroups.Count < originalGroupCount;
        }
    }
}
