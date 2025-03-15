using Printpress.Domain.Entities;
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
                 page.Margin(40);

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
            column.Item().Row(row =>
            {
                row.RelativeItem().AlignLeft().Column(col =>
                {

                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span($"{DateTime.Now:d}");
                        });

                        row.AutoItem().Text(text =>
                        {
                            text.Span(" : تاريخ الطباعه").SemiBold();
                        });
                    });

                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span(Model.Client.Mobile);
                        });

                        row.AutoItem().Text(text =>
                        {
                            text.Span(" : تيلفون العميل").SemiBold();
                        });
                    });

                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span(Model.Client.Name);
                        });

                        row.AutoItem().Text(text =>
                        {
                            text.Span(" : أسم العميل").SemiBold();
                        });
                    });

                    col.Item().PaddingVertical(2).AlignRight().Row(row =>
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span(Model.Client.Address);
                        });

                        row.AutoItem().Text(text =>
                        {
                            text.Span(" : عنوان العميل").SemiBold();
                        });
                    });


                });
            });
        });
    }
    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {

            foreach (var group in Model.OrderGroups)
            {
                column.Spacing(5);
                column.Item().AlignCenter().Text(group.Name);

                column.Item().Element(x => ComposeTable(x, group.Items));

                column.Spacing(5);
            }


            column.Item().PaddingTop(25).Element(ComposeComments);
        });
    }
    void ComposeTable(IContainer container, List<Item> Items)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).AlignCenter().Text("الاجمالي");
                header.Cell().Element(CellStyle).AlignCenter().Text("الكميه");
                header.Cell().Element(CellStyle).AlignCenter().Text("سعر الوحده");
                header.Cell().Element(CellStyle).AlignCenter().Text("النوع");
                header.Cell().Element(CellStyle).AlignCenter().Text("#");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).Border(1).BorderColor(Colors.Black).PaddingVertical(8);
                }
            });



            foreach (var item in Items)
            {
                table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Price * item.Quantity}");
                table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Price}");
                table.Cell().Element(CellStyle).AlignCenter().Text(item.Name);
                table.Cell().Element(CellStyle).AlignCenter().Text((Items.IndexOf(item) + 1).ToString());


                static IContainer CellStyle(IContainer container)
                {
                    return container.Border(1).BorderColor(Colors.Black).PaddingVertical(5);
                }
            }
        });
    }
    void ComposeComments(IContainer container)
    {
        container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text("Comments").FontSize(14);
            column.Item().Text("");
        });
    }

}


