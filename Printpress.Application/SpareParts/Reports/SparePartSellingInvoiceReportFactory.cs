using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Printpress.Domain;
using QuestPDF.Infrastructure;

namespace Printpress.Application;

internal class SparePartSellingInvoiceReportFactory(IUnitOfWork unitOfWork, IConfiguration configuration) : IReportFactory
{
    public string ReportName => "sparePartSellingInvoice";

    public async Task<IDocument> GenerateReport(NameValueCollection queryParams)
    {
        Guid id = Guid.Parse(queryParams.GetValues("id").FirstOrDefault());

        QuestPDF.Settings.License = LicenseType.Community;

        string[] includes =
        [
            nameof(SparePartSellingInvoice.SparePartSellingInvoiceLines),
            $"{nameof(SparePartSellingInvoice.SparePartSellingInvoiceLines)}.{nameof(SparePartSellingInvoiceLine.InventoryItem)}"
        ];

        var invoice = await unitOfWork.SparePartSellingInvoiceRepository.FirstOrDefaultAsync(
            sellingInvoice => sellingInvoice.Id == id,
            true,
            includes);

        if (invoice is null)
            return null;

        return new SparePartSellingInvoiceReport(invoice, configuration);
    }
}
