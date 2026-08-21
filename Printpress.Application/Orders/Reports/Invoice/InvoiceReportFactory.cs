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

            var document = new InvoiceReport(model, configuration, isPartial);

            return document;

        }

        private static bool ApplyGroupFilter(Order model, string groupIdsRaw)
        {
            if (model is null || string.IsNullOrWhiteSpace(groupIdsRaw))
                return false;

            var selectedGroupIds = groupIdsRaw
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => Guid.TryParse(value, out var id) ? id : (Guid?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToHashSet();

            if (selectedGroupIds.Count == 0)
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

            var isPartial = model.OrderGroups.Count < originalGroupCount;
            if (!isPartial)
                return false;

            model.SellingItems = [];
            model.TotalPrice = model.OrderGroups
                .SelectMany(g => g.Items ?? [])
                .Where(i => !i.IsDeleted)
                .Sum(i => i.Price * i.Quantity);

            return true;
        }
    }
}
