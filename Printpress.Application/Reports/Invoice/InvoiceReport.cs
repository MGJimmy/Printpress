using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Printpress.Domain.Entities;
using Printpress.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Printpress.Application;

public class InvoiceReport : IDocument
{
    public required Order Model { get; set; }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    private Color BrownColor = Color.FromRGB(166, 101, 9);

    public void Compose(IDocumentContainer container)
    {
        container
             .Page(page =>
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
                            text.Span("مطبعة وادي النيل").ExtraBold().FontSize(20);
                        });
                    });
                });


                row.RelativeItem().AlignLeft().AlignMiddle().Column(col =>
                {
                    col.Item().AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("ت: 01125355907").SemiBold();
                        });
                    });
                });
               
            });



            column.Item().PaddingHorizontal(5).AlignCenter().Text("فاتورة مبيعات")
                                .FontSize(20).SemiBold().FontColor(BrownColor);


            column.Item().PaddingHorizontal(40).Row(row =>
            {
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {

                        row.AutoItem().Text(text =>
                        {
                            text.Span("أسم العميل : ").SemiBold();
                        });
                        row.AutoItem().Text(text =>
                        {
                            text.Span(Model.Client.Name);
                        });
                    });

                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {

                        row.AutoItem().Text(text =>
                        {
                            text.Span("تيلفون العميل : ").SemiBold();
                        });
                        row.AutoItem().Text(text =>
                        {
                            text.Span(Model.Client.Mobile);
                        });

                    });


                });

                row.RelativeItem().AlignLeft().Column(col =>
                {
                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("تاريخ الطباعه : ").SemiBold();
                        });
                        row.AutoItem().Text(text =>
                        {
                            text.Span($"{DateTime.Now:d}");
                        });
                    });

                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span(" عنوان العميل : ").SemiBold();
                        });
                        row.AutoItem().Text(text =>
                        {
                            text.Span(Model.Client.Address);
                        });
                    });

                });

            });


        });
    }
    void ComposeContent(IContainer container)
    {
        container.Padding(40).Column(column =>
        {
            decimal total = 0;

            foreach (var group in Model.OrderGroups)
            {
                column.Spacing(5);
                column.Item().PaddingHorizontal(30).PaddingVertical(10).Element(x => ComposeTable(x, group));
                column.Spacing(5);
                total += group.Items.Sum(item => item.Price * item.Quantity);

            }

            AddTotles(column);
            AddNames(column);
        });
    }

    void AddTotles(ColumnDescriptor column)
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

                AddText("الأجمالي الكلي  :", 10, true);
                AddText(Model.TotalPrice.ToString());
                AddText("المدفوع  :", 40, true);
                AddText(Model.TotalPaid.ToString());
                AddText("الباقي  :", 40, true);
                AddText((Model.TotalPrice - Model.TotalPaid).ToString());
            });

    }

    void AddNames(ColumnDescriptor column)
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

    void ComposeTable(IContainer container, OrderGroup orderGroup)
    {
        var servicesNames = string.Join(',', orderGroup.OrderGroupServices.Select(orderGroupService => orderGroupService.Service.Name));
        var groupHeader = $"{orderGroup.Name} : ({servicesNames})";
        var isPrinting = IsPrintingService(orderGroup);
        uint columnCount = (uint)(isPrinting ? 7 : 5); // Adjusted based on whether printing columns are present

        container.Table(table =>
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

            table.Header(header =>
            {
                header.Cell().ColumnSpan(columnCount).Background(BrownColor).Element(CellStyle).AlignCenter().Text(groupHeader).FontColor(Colors.White);

                header.Cell().Element(CellStyle).AlignCenter().Text("#");
                header.Cell().Element(CellStyle).AlignCenter().Text("النوع");
                header.Cell().Element(CellStyle).AlignCenter().Text("سعر الوحده");

                if (isPrinting)
                {
                    header.Cell().Element(CellStyle).AlignCenter().Text("عدد الصفحات");
                    header.Cell().Element(CellStyle).AlignCenter().Text("عدد الاوجه");
                }

                header.Cell().Element(CellStyle).AlignCenter().Text("الكميه");
                header.Cell().Element(CellStyle).AlignCenter().Text("الاجمالي");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).Border(1, Unit.Mil).BorderColor(Colors.Black).PaddingVertical(8);
                }
            });

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

                static IContainer CellStyle(IContainer container)
                {
                    return container.Border(1, Unit.Mil).BorderColor(Colors.Black).PaddingVertical(5);
                }
            }

            table.Cell().ColumnSpan(columnCount - 1).Element(FooterCellStyle).AlignCenter().Text("إجمالي المجموعه");
            table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{orderGroup.Items.Sum(item => item.Price * item.Quantity)}");

            static IContainer FooterCellStyle(IContainer container)
            {
                return container.DefaultTextStyle(x => x.SemiBold())
                               .Border(7, Unit.Mil)
                               .BorderColor(Colors.Black)
                               .PaddingVertical(8)
                               .Background(Colors.Grey.Lighten5);
            }


        });

        // need to move to orderGroup service
        static bool IsPrintingService(OrderGroup orderGroup)
        {
            return orderGroup.OrderGroupServices.Any(grbService => grbService.Service.ServiceCategory == ServiceCategoryEnum.Printing);
        }
    }


}


