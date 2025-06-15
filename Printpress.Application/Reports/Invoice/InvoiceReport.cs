using Microsoft.Extensions.Configuration;
using Printpress.Domain.Entities;
using Printpress.Domain.Enums;
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

    /// <summary>
    /// Gets or sets the order model for the invoice
    /// </summary>
    private Order _model { get; set; }

    public InvoiceReport(Order model, IConfiguration configuration)
    {
        _companyName = configuration.GetRequiredSection("CompanyInfo:Name").Value;
        _companyPhone = configuration.GetRequiredSection("CompanyInfo:Phone").Value;
        _model = model;
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
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span(_companyName).ExtraBold().FontSize(20);
                        });
                    });
                });


                row.RelativeItem().AlignLeft().AlignMiddle().Column(col =>
                {
                    col.Item().AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            
                            text.Span("ت: " + _companyPhone).SemiBold();
                        });
                    });
                });

            });

            // Title
            column.Item()
                  .PaddingHorizontal(5)
                  .AlignCenter()
                  .Text("فاتورة مبيعات")
                  .FontSize(20)
                  .SemiBold();

            // Client Information
            column.Item()
                  .PaddingHorizontal(40)
                  .Row(row =>
                  {
                      // Right Column - Print Date and Address
                      row.RelativeItem()
                         .AlignRight()
                         .Column(col => ComposeClientInfoRightColumn(col));

                      // Left Column - Client Name and Phone
                      row.RelativeItem()
                         .AlignLeft()
                         .Column(col => ComposeClientInfoLeftColumn(col));
                  });


        });
    }

    private void ComposeClientInfoRightColumn(ColumnDescriptor col)
    {
        col.Item()
           .PaddingVertical(2)
           .AlignRight()
           .Row(row =>
           {
               row.AutoItem()
                  .Text(text => text.Span("تاريخ الطباعه : ").SemiBold());
               row.AutoItem()
                  .Text(text => text.Span($"{DateTime.Now:d}"));
           });

        col.Item()
           .PaddingVertical(2)
           .AlignRight()
           .Row(row =>
           {
               row.AutoItem()
                  .Text(text => text.Span(" عنوان العميل :").SemiBold());
               row.AutoItem()
                  .Text(text => text.Span(_model.Client.Address));
           });
    }

    private void ComposeClientInfoLeftColumn(ColumnDescriptor col)
    {
        col.Item()
           .PaddingVertical(2)
           .AlignRight()
           .Row(row =>
           {
               row.AutoItem()
                  .Text(text => text.Span("أسم العميل : ").SemiBold());
               row.AutoItem()
                  .Text(text => text.Span(_model.Client.Name));
           });

        col.Item()
           .PaddingVertical(2)
           .AlignRight()
           .Row(row =>
           {
               row.AutoItem()
                  .Text(text => text.Span("تيلفون العميل : ").SemiBold());
               row.AutoItem()
                  .Text(text => text.Span(_model.Client.Mobile));
           });
    }

    private void ComposeContent(IContainer container)
    {
        container.Padding(40)
                 .Column(column =>
                 {

                     AddServicesPrices(column, _model.Services.Select(s => s.Service));

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

    private void AddServicesPrices(ColumnDescriptor column, IEnumerable<Service> services)
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

              foreach (var service in services)
              {
                  table.Cell().Element(CellStyle).Text(service.Name).AlignCenter();
                  table.Cell().Element(CellStyle).Text(service.Price.ToString()).AlignCenter();
              }


          });
      });

    }

    private void AddTotles(ColumnDescriptor column)
    {
        column.Item().PaddingHorizontal(30).PaddingTop(30).Border(1).Row(row =>
        {
            void AddText(string content, int paddingRight = 5, bool isBold = false)
            {
                var text = row.AutoItem().PaddingVertical(10).PaddingRight(paddingRight).Text(content);
                if (isBold)
                    text.ExtraBold();
                else
                    text.SemiBold();
            }

            var totalPaid = _model.TotalPaid ?? 0;

            AddText("الأجمالي الكلي  :", 10, true);
            AddText(_model.TotalPrice.ToString());
            AddText("المدفوع  :", 40, true);
            AddText(totalPaid.ToString());
            AddText("الباقي  :", 40, true);
            AddText((_model.TotalPrice - totalPaid).ToString());
        });

    }

    private void AddDeliveryDetails(ColumnDescriptor column)
    {
        column.Item().PaddingHorizontal(30).PaddingTop(10).Border(1).Row(row =>
        {
            void AddText(string content, int paddingRight = 5, bool isBold = false)
            {
                var text = row.AutoItem().PaddingVertical(10).PaddingRight(paddingRight).Text(content);
                if (isBold)
                    text.ExtraBold();
                else
                    text.SemiBold();
            }

            AddText("اسم المسلم  :", 10, true);
            AddText("التوقيع  :", 110, true);
            AddText("التاريخ  :", 110, true);
        });

        column.Item().PaddingHorizontal(30).PaddingTop(10).Border(1).Row(row =>
        {
            void AddText(string content, int paddingRight = 5, bool isBold = false)
            {
                var text = row.AutoItem().PaddingVertical(10).PaddingRight(paddingRight).Text(content);
                if (isBold)
                    text.ExtraBold();
                else
                    text.SemiBold();
            }

            AddText("اسم المستلم  :", 10, true);
            AddText("التوقيع  :", 110, true);
            AddText("التاريخ  :", 110, true);
        });

    }


    private void ComposeTable(IContainer container, OrderGroup orderGroup)
    {
        var servicesNames = string.Join(',', orderGroup.OrderGroupServices.Select(s => s.Service.Name));
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
            grbService.Service.ServiceCategory == ServiceCategoryEnum.Printing);
    }
}


