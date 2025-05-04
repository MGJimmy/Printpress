using Printpress.Domain.Entities;
using Printpress.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Printpress.Application;

public class InvoiceDocument : IDocument
{
    public required Order Model { get; set; }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

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
            column.Item().PaddingHorizontal(5).AlignCenter().Text("فاتوره")
                                .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);



            column.Item().PaddingHorizontal(40).Row(row =>
            {
                row.RelativeItem().AlignRight().Column(col =>
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
                            text.Span(" عنوان العميل :").SemiBold();
                        });
                        row.AutoItem().Text(text =>
                        {
                            text.Span(Model.Client.Address);
                        });
                    });

                });

                row.RelativeItem().AlignLeft().Column(col =>
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

            column.Item().Padding(30).Border(1).Row(row =>
            {
                row.AutoItem().Padding(10).Text("الأجمالي الكلي  :").SemiBold();
                row.AutoItem().PaddingVertical(10).Text(total.ToString()).SemiBold();
            });

        });
    }
    void ComposeTable(IContainer container, OrderGroup orderGroup)
    {
        var servicesNames = string.Join(',', orderGroup.OrderGroupServices.Select(orderGroupService => orderGroupService.Service.Name).ToList());
        var groubHeader = $"{orderGroup.Name} + ({servicesNames})";

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();

                if (IsPrintingService(orderGroup))
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                }

            });

            table.Header(header =>
            {
                header.Cell().ColumnSpan(7).Element(CellStyle).AlignCenter().Text(groubHeader).FontColor(Colors.Blue.Darken2);
                header.Cell().Element(CellStyle).AlignCenter().Text("#");
                header.Cell().Element(CellStyle).AlignCenter().Text("النوع");
                header.Cell().Element(CellStyle).AlignCenter().Text("سعر الوحده");

                if (IsPrintingService(orderGroup))
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

                if (IsPrintingService(orderGroup))
                {
                    var noOfPages = item.Details.FirstOrDefault(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPages)?.Value;
                    var numberOfPrintingFaces = item.Details.FirstOrDefault(x => x.ItemDetailsKey == ItemDetailsKeyEnum.NumberOfPrintingFaces)?.Value;

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

            table.Cell().ColumnSpan(6).Element(FooterCellStyle).AlignCenter().Text("إجمالي المجموعه");
            table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{orderGroup.Items.Sum(item => item.Price * item.Quantity)}");

            static IContainer FooterCellStyle(IContainer container)
            {
                return container.DefaultTextStyle(x => x.SemiBold())
                               .Border(7, Unit.Mil)
                               .BorderColor(Colors.Black)
                               .PaddingVertical(8)
                               .Background(Colors.Grey.Lighten5);
            }

            static bool IsPrintingService(OrderGroup orderGroup)
            {
                return orderGroup.OrderGroupServices.Any(grbService => grbService.Service.ServiceCategory == ServiceCategoryEnum.Printing);
            }

        });




    }





}


