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
                     nameof(Order.SellingItems),
                     $"{nameof(Order.SellingItems)}.{nameof(OrderSellingItem.InventoryItem)}"
            ];

            var model = await unitOfWork.OrderRepository.FirstOrDefaultAsync(order => order.Id == id, true, includes);

            var document = new InvoiceReport(model, configuration);

            return document;

        }
    }
}
