using Microsoft.Extensions.Configuration;
using Printpress.Application;
using Printpress.Domain;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

// Run with: dotnet run --project tests/InvoiceLayoutChecks -- <optional image output directory>
QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.EnableDebugging = true;

var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
{
    ["CompanyInfo:Name"] = "مطبعة وادي النيل",
    ["CompanyInfo:Phone"] = "01022775500"
}).Build();

Order CreateOrder() => new()
{
    Name = "اختبار عرض الفاتورة",
    Client = new Client { Name = "عميل الاختبار" },
    TotalPrice = 70584.00m,
    TotalPaid = 100000m,
    OrderGroups = [],
    Services = [],
    SellingItems = [new OrderSellingItem
    {
        Name = "ورق كوشيه", Details = "تفاصيل عنصر البيع", Quantity = 1, Price = 70584.00m
    }]
};

var scenarios = new List<(string Name, Order Order, bool Partial, IConfiguration Config)>();
scenarios.Add(("reported-credit", CreateOrder(), false, configuration));
var unpaid = CreateOrder();
unpaid.TotalPaid = 0;
scenarios.Add(("unpaid", unpaid, false, configuration));
scenarios.Add(("partial", CreateOrder(), true, configuration));
var large = CreateOrder();
large.TotalPrice = 123456789012345678.12m;
large.TotalPaid = 234567890123456789.34m;
large.SellingItems[0].Price = large.TotalPrice.Value;
large.SellingItems[0].Name = string.Join(" ", Enumerable.Repeat("اسم عنصر طويل", 8));
large.SellingItems[0].Details = string.Join("\n", Enumerable.Repeat("تفاصيل طويلة لاختبار التفاف النص داخل العمود", 12));
var longHeader = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
{
    ["CompanyInfo:Name"] = "مطبعة وادي النيل للطباعة والنشر والتوزيع والخدمات المتكاملة",
    ["CompanyInfo:Phone"] = "01022775500 / 01234567890"
}).Build();
scenarios.Add(("long-text-large-values", large, false, longHeader));

var groups = CreateOrder();
foreach (var code in new[] { "Printing", "Other" })
{
    var service = new Service { Name = "خدمة الطباعة والتجهيز", ServiceCategory = new ServiceCategory { Code = code } };
    groups.Services.Add(new OrderService { Service = service, Price = 12345.67m });
    groups.OrderGroups.Add(new OrderGroup
    {
        Name = "مجموعة اختبار التفاف النص",
        OrderGroupServices = [new OrderGroupService { Service = service }],
        Items = Enumerable.Range(1, 35).Select(i => new OrderItem
        {
            Name = $"عنصر رقم {i} باسم طويل لاختبار عرض الأعمدة",
            Quantity = 1000, Price = 12345.67m, Details = []
        }).ToList()
    });
}
scenarios.Add(("multipage-printing-and-other-groups", groups, false, configuration));

foreach (var scenario in scenarios)
{
    var report = new InvoiceReport(scenario.Order, scenario.Config, scenario.Partial);
    var pdf = report.GeneratePdf();
    if (pdf.Length == 0) throw new Exception($"Empty PDF: {scenario.Name}");
    if (args.Length > 0)
    {
        Directory.CreateDirectory(args[0]);
        var page = 0;
        foreach (var bytes in report.GenerateImages(new ImageGenerationSettings { RasterDpi = 100 }))
            File.WriteAllBytes(Path.Combine(args[0], $"{scenario.Name}-{++page}.png"), bytes);
    }
    Console.WriteLine($"PASS: {scenario.Name}");
}
