using Microsoft.Extensions.Configuration;
using Printpress.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Printpress.Application;

/// <summary>
/// Represents an invoice report document for orders
/// </summary>
public class InvoiceReport : IDocument
{
    private readonly string _companyName;
    private readonly string _companyPhone;
    private readonly bool _isPartial;

    /// <summary>
    /// Gets or sets the order model for the invoice
    /// </summary>
    private Order _model { get; set; }

    public InvoiceReport(Order model, IConfiguration configuration, bool isPartial = false)
    {
        _companyName = configuration.GetRequiredSection("CompanyInfo:Name").Value;
        _companyPhone = configuration.GetRequiredSection("CompanyInfo:Phone").Value;
        _model = model;
        _isPartial = isPartial;
    }

    /// <inheritdoc />
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    /// <inheritdoc />
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    /// <inheritdoc />
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
                row.Spacing(12);
                row.RelativeItem(2).Text(_companyName).ExtraBold().FontSize(20);
                row.RelativeItem().AlignMiddle().Text("ت: " + _companyPhone).SemiBold();
            });

            // Client Information
            column.Item().PaddingHorizontal(40).PaddingTop(6).Text(text =>
            {
                text.Span("اسم الطلبية : ").SemiBold();
                text.Span(_model.Name);
                if (_model.IsZeroOrder)
                    text.Span("  (طلب صفري)").SemiBold().FontColor(Colors.Orange.Darken2);
            });

            column.Item().PaddingHorizontal(40).PaddingVertical(4).Text(text =>
            {
                text.Span("اسم العميل : ").SemiBold();
                text.Span(_model.Client.Name);
            });

            column.Item().PaddingHorizontal(40).PaddingBottom(6).Text(text =>
            {
                text.Span("تاريخ الطباعة : ").SemiBold();
                text.Span($"{DateTime.Now:d}");
            });


        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Padding(40)
                 .Column(column =>
                 {

                     if (_model.OrderGroups?.Count > 0 && _model.Services?.Count > 0)
                         AddServicesPrices(column, _model.Services);

                     if (_model.SellingItems?.Count > 0)
                         AddSellingItems(column, _model.SellingItems);

                     // group section
                     foreach (var group in _model.OrderGroups)
                     {
                         column.Spacing(5);
                         column.Item()
                               .PaddingHorizontal(30)
                               .PaddingVertical(10)
                               .Element(x => ComposeTable(x, group));
                         column.Spacing(5);
                     }

                     AddTotles(column);
                     AddDeliveryDetails(column);
                 });
    }

    private void AddServicesPrices(ColumnDescriptor column, IEnumerable<OrderService> orderServices)
    {
        column.Item()
      .PaddingHorizontal(30)
      .PaddingVertical(10)
      .Element(x =>
      {
          x.Table(table =>
          {
              table.ColumnsDefinition(columns =>
              {
                  columns.RelativeColumn(); // Service Name
                  columns.RelativeColumn(); // Price
              });
              table.Header(header =>
              {
                  header.Cell().Element(CellStyle).Text("الخدمه").SemiBold().AlignCenter();
                  header.Cell().Element(CellStyle).Text("السعر").SemiBold().AlignCenter();
              });

              foreach (var orderService in orderServices)
              {
                  var displayName = orderService.IsCover
                      ? $"غلاف {orderService.Service.Name}"
                      : orderService.Service.Name;
                  table.Cell().Element(CellStyle).Text(displayName).AlignCenter();
                  table.Cell().Element(CellStyle).Text(orderService.Price.ToString()).AlignCenter();
              }


          });
      });

    }

    private void AddSellingItems(ColumnDescriptor column, IEnumerable<OrderSellingItem> sellingItems)
    {
        column.Item()
              .PaddingHorizontal(30)
              .PaddingVertical(10)
              .Element(x =>
              {
                  x.Table(table =>
                  {
                      table.ColumnsDefinition(columns =>
                      {
                          columns.RelativeColumn(); // #
                          columns.RelativeColumn(2); // اسم العنصر
                          columns.RelativeColumn(2); // تفاصيل
                          columns.RelativeColumn(); // الكمية
                          columns.RelativeColumn(); // السعر
                          columns.RelativeColumn(); // الإجمالي
                      });

                      table.Header(header =>
                      {
                          header.Cell().ColumnSpan(6).Element(CellStyle).AlignCenter().Text("عناصر البيع").SemiBold();
                          header.Cell().Element(CellStyle).AlignCenter().Text("#").SemiBold();
                          header.Cell().Element(CellStyle).AlignCenter().Text("اسم العنصر").SemiBold();
                          header.Cell().Element(CellStyle).AlignCenter().Text("تفاصيل").SemiBold();
                          header.Cell().Element(CellStyle).AlignCenter().Text("الكمية").SemiBold();
                          header.Cell().Element(CellStyle).AlignCenter().Text("السعر").SemiBold();
                          header.Cell().Element(CellStyle).AlignCenter().Text("الإجمالي").SemiBold();
                      });

                      int index = 1;
                      foreach (var item in sellingItems)
                      {
                          var itemName = item.IsInventoryItem ? item.InventoryItem?.Name ?? item.Name : item.Name;

                          table.Cell().Element(CellStyle).AlignCenter().Text(index.ToString()).FontSize(12);
                          table.Cell().Element(CellStyle).AlignCenter().Text(itemName).FontSize(12);
                          table.Cell().Element(CellStyle).AlignCenter().Text(item.Details ?? string.Empty).FontSize(12);
                          table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString()).FontSize(12);
                          table.Cell().Element(CellStyle).AlignCenter().Text(item.Price.ToString()).FontSize(12);
                          table.Cell().Element(CellStyle).AlignCenter().Text((item.Price * (decimal)item.Quantity).ToString()).FontSize(12);
                          index++;
                      }

                      table.Cell().ColumnSpan(5).Element(FooterCellStyle).AlignCenter().Text("إجمالي عناصر البيع");
                      table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{sellingItems.Sum(i => i.Price * (decimal)i.Quantity)}");
                  });
              });
    }

    private void AddTotles(ColumnDescriptor column)
    {
        column.Item().PaddingHorizontal(30).PaddingTop(30).Border(1).Row(row =>
        {
            void AddTotal(string label, decimal value)
            {
                row.RelativeItem().Padding(8).Column(cell =>
                {
                    cell.Item().Text(label).ExtraBold();
                    cell.Item().PaddingTop(4).Text(value.ToString()).SemiBold();
                });
            }

            AddTotal("الإجمالي الكلي", _model.TotalPrice ?? 0);

            if (_isPartial)
                return;

            var totalPaid = _model.TotalPaid ?? 0;
            AddTotal("المدفوع", totalPaid);
            var remaining = (_model.TotalPrice ?? 0) - totalPaid;
            AddTotal(remaining >= 0 ? "الباقي" : "عربون / رصيد", Math.Abs(remaining));
        });
    }

    private void AddDeliveryDetails(ColumnDescriptor column)
    {
        AddSignatureRow(column, "اسم المسلم :");
        AddSignatureRow(column, "اسم المستلم :");
    }

    private static void AddSignatureRow(ColumnDescriptor column, string nameLabel)
    {
        column.Item().PaddingHorizontal(30).PaddingTop(10).Border(1).Row(row =>
        {
            row.RelativeItem().Padding(10).Text(nameLabel).ExtraBold();
            row.RelativeItem().Padding(10).Text("التوقيع :").ExtraBold();
            row.RelativeItem().Padding(10).Text("التاريخ :").ExtraBold();
        });
    }

    private void ComposeTable(IContainer container, OrderGroup orderGroup)
    {
        var servicesNames = string.Join(',', orderGroup.OrderGroupServices.Select(FormatGroupServiceName));
        var groupHeader = $"{orderGroup.Name}: ({servicesNames})";
        var isPrinting = IsPrintingService(orderGroup);
        uint columnCount = (uint)(isPrinting ? 7 : 5);

        container.Table(table =>
        {
            DefineTableColumns(table, isPrinting);
            ComposeTableHeader(table, groupHeader, isPrinting, columnCount);
            ComposeTableBody(table, orderGroup, isPrinting);
            ComposeTableFooter(table, orderGroup, columnCount);
        });
    }

    private static void DefineTableColumns(TableDescriptor table, bool isPrinting)
    {
        table.ColumnsDefinition(columns =>
        {
            columns.RelativeColumn(); // #
            columns.RelativeColumn(); // النوع
            columns.RelativeColumn(); // سعر الوحده

            if (isPrinting)
            {
                columns.RelativeColumn(); // عدد الصفحات
                columns.RelativeColumn(); // عدد الاوجه
            }

            columns.RelativeColumn(); // الكميه
            columns.RelativeColumn(); // الاجمالي
        });
    }

    private static void ComposeTableHeader(TableDescriptor table, string groupHeader, bool isPrinting, uint columnCount)
    {
        table.Header(header =>
        {
            header.Cell()
                  .ColumnSpan(columnCount)
                  .Element(CellStyle)
                  .AlignCenter()
                  .Text(groupHeader);

            header.Cell().Element(CellStyle).AlignCenter().Text("#");
            header.Cell().Element(CellStyle).AlignCenter().Text("اسم العنصر");
            header.Cell().Element(CellStyle).AlignCenter().Text("السعر");

            if (isPrinting)
            {
                header.Cell().Element(CellStyle).AlignCenter().Text("عدد الصفحات");
                header.Cell().Element(CellStyle).AlignCenter().Text("عدد الاوجه");
            }

            header.Cell().Element(CellStyle).AlignCenter().Text("الكميه");
            header.Cell().Element(CellStyle).AlignCenter().Text("الاجمالي");
        });
    }

    private static void ComposeTableBody(TableDescriptor table, OrderGroup orderGroup, bool isPrinting)
    {
        foreach (var item in orderGroup.Items)
        {
            table.Cell().Element(CellStyle).AlignCenter().Text((orderGroup.Items.IndexOf(item) + 1).ToString()).FontSize(12);
            table.Cell().Element(CellStyle).AlignCenter().Text(item.Name).FontSize(12);
            table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Price}").FontSize(12);

            if (isPrinting)
            {
                var noOfPages = item.Details.FirstOrDefault(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages)?.Value ?? "";
                var numberOfPrintingFaces = item.Details.FirstOrDefault(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces)?.Value ?? "";

                table.Cell().Element(CellStyle).AlignCenter().Text(noOfPages).FontSize(12);
                table.Cell().Element(CellStyle).AlignCenter().Text(numberOfPrintingFaces).FontSize(12);
            }

            table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString()).FontSize(12);
            table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Price * item.Quantity}").FontSize(12);
        }
    }

    private static void ComposeTableFooter(TableDescriptor table, OrderGroup orderGroup, uint columnCount)
    {
        table.Cell()
             .ColumnSpan(columnCount - 1)
             .Element(FooterCellStyle)
             .AlignCenter()
             .Text("إجمالي المجموعه");

        table.Cell()
             .Element(FooterCellStyle)
             .AlignCenter()
             .Text($"{orderGroup.Items.Sum(item => item.Price * item.Quantity)}");
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.Border(1, Unit.Mil)
                        .BorderColor(Colors.Black)
                        .PaddingVertical(5);
    }

    private static IContainer FooterCellStyle(IContainer container)
    {
        return container.DefaultTextStyle(x => x.SemiBold())
                        .Border(7, Unit.Mil)
                        .BorderColor(Colors.Black)
                        .PaddingVertical(8)
                        .Background(Colors.Grey.Lighten5);
    }

    private static bool IsPrintingService(OrderGroup orderGroup)
    {
        return orderGroup.OrderGroupServices.Any(grbService =>
            grbService.Service.ServiceCategory?.Code == "Printing");
    }

    private static string FormatGroupServiceName(OrderGroupService groupService)
    {
        var name = groupService.Service?.Name ?? string.Empty;
        return groupService.IsCover ? $"غلاف {name}" : name;
    }
}


