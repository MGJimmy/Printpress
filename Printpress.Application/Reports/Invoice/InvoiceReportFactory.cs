using System.Collections.Specialized;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Printpress.Domain.Entities;
using QuestPDF.Infrastructure;

namespace Printpress.Application
{
    internal class InvoiceReportFactory(IUnitOfWork unitOfWork, IConfiguration configuration) : IReportFactory
    {
        public string ReportName => "invoice";
        public async Task<IDocument> GenerateReport(NameValueCollection queryParams)
        {
            int id = int.Parse(queryParams.GetValues("id").FirstOrDefault());

            QuestPDF.Settings.License = LicenseType.Community;

            string[] includes = [
                     nameof(Order.Client),
                     nameof(Order.Services),
                     $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}.{nameof(Item.Details)}",
                     $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}.{nameof(OrderGroupService.Service)}"
            ];

            var model = await unitOfWork.OrderRepository.FirstOrDefaultAsync(order => order.Id == id, true, includes);

            var document = new InvoiceReport(model, configuration);

            return document;

        }
    }
}
