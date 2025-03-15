using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Printpress.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class Report(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet]
        [Route("generateInvoiceReport")]
        public async Task<IActionResult> GenerateInvoiceReport()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var model = await _unitOfWork.OrderRepository.FirstOrDefaultAsync(order => order.Id == 1, true,["Client", "OrderGroups", "OrderGroups.Items" ]);

            var document = new InvoiceDocument{ Model = model };

            document.GeneratePdfAndShow();

            return Ok();

            //var tempFilePath = Path.GetTempFileName();

            //document.GeneratePdf(tempFilePath);

            //return File(new FileStream(tempFilePath, FileMode.Open), "application/pdf", "invoice.pdf");

        }
    }
}
