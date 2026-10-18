using Microsoft.Extensions.Configuration;
using Printpress.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Printpress.Application;

public class SparePartSellingInvoiceReport : IDocument
{
    private readonly string _companyName;
    private readonly string _companyPhone;
    private readonly SparePartSellingInvoice _model;

    public SparePartSellingInvoiceReport(SparePartSellingInvoice model, IConfiguration configuration)
    {
        _companyName = configuration.GetRequiredSection("CompanyInfo:Name").Value;
        _companyPhone = configuration.GetRequiredSection("CompanyInfo:Phone").Value;
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.ContentFromRightToLeft();
            page.Margin(10);
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingHorizontal(40).PaddingVertical(10).Row(row =>
            {
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text(text =>
                    {
                        text.Span(_companyName).ExtraBold().FontSize(20);
                    });
                });

                row.RelativeItem().AlignLeft().AlignMiddle().Column(col =>
                {
                    col.Item().AlignRight().Text(text =>
                    {
                        text.Span("ت: " + _companyPhone).SemiBold();
                    });
                });
            });

            column.Item().PaddingHorizontal(40).AlignCenter().Text("فاتورة بيع قطع غيار").ExtraBold().FontSize(18);

            if (_model.IsVoided)
            {
                column.Item().PaddingHorizontal(40).PaddingTop(4).AlignCenter()
                    .Text("ملغاة").ExtraBold().FontSize(16).FontColor(Colors.Red.Darken2);
            }

            column.Item().PaddingHorizontal(40).PaddingTop(6).Text(text =>
            {
                text.Span("رقم الفاتورة : ").SemiBold();
                text.Span(_model.InvoiceNumber.ToString());
            });

            column.Item().PaddingHorizontal(40).PaddingVertical(4).Text(text =>
            {
                text.Span("تاريخ الفاتورة : ").SemiBold();
                text.Span($"{_model.InvoiceDate:yyyy-MM-dd}");
            });

            column.Item().PaddingHorizontal(40).PaddingBottom(6).Text(text =>
            {
                text.Span("اسم العميل : ").SemiBold();
                text.Span(_model.ClientName);
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Padding(40).Column(column =>
        {
            column.Item().PaddingHorizontal(30).PaddingVertical(10).Element(ComposeLines);
            column.Item().PaddingHorizontal(30).PaddingTop(30).Border(1).Row(row =>
            {
                row.AutoItem().PaddingVertical(10).PaddingRight(10).Text("الإجمالي الكلي  :").ExtraBold();
                row.AutoItem().PaddingVertical(10).PaddingRight(5).Text(_model.TotalAmount.ToString()).SemiBold();
            });
        });
    }

    private void ComposeLines(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).AlignCenter().Text("اسم القطعة").SemiBold();
                header.Cell().Element(CellStyle).AlignCenter().Text("الكمية").SemiBold();
                header.Cell().Element(CellStyle).AlignCenter().Text("سعر الوحدة").SemiBold();
                header.Cell().Element(CellStyle).AlignCenter().Text("الإجمالي").SemiBold();
            });

            foreach (var line in _model.SparePartSellingInvoiceLines)
            {
                table.Cell().Element(CellStyle).AlignCenter().Text(line.InventoryItem?.Name ?? string.Empty).FontSize(12);
                table.Cell().Element(CellStyle).AlignCenter().Text(line.Quantity.ToString()).FontSize(12);
                table.Cell().Element(CellStyle).AlignCenter().Text(line.UnitPrice.ToString()).FontSize(12);
                table.Cell().Element(CellStyle).AlignCenter().Text(line.LineTotal.ToString()).FontSize(12);
            }
        });
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.Border(1, Unit.Mil)
                        .BorderColor(Colors.Black)
                        .PaddingVertical(5);
    }
}
