using System.Collections.Specialized;
using QuestPDF.Fluent;

namespace Printpress.API;

[Route("api/[controller]")]

public class Report(IEnumerable<IReportFactory> reportFactories) : AppBaseController
{

    [HttpGet]
    [Route("generateReport")]
    public async Task<IActionResult> GenerateRepor()
    {

        NameValueCollection queryParams = new NameValueCollection();

        foreach (var pair in Request.Query)
        {
            queryParams.Add(pair.Key, pair.Value);
        }

        var reportFactory = reportFactories.FirstOrDefault(x => x.ReportName == queryParams["reportName"]);

        if (reportFactory == null)
        {
            return NotFound("Report not found");
        }
        var document = await reportFactory.GenerateReport(queryParams);
        if (document == null)
        {
            return NotFound("Document not found");
        }
        // Generate the PDF and return it as a file
        var result = new FileContentResult(document.GeneratePdf(), "application/pdf")
        {
            FileDownloadName = $"{queryParams["reportName"]}.pdf"
        };

        return result;

    }
}

