using Microsoft.AspNetCore.Mvc;
using Printpress.Application;
using Printpress.Domain.Entities;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

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

            string[] includes = [nameof(Order.Client), nameof(Order.OrderGroups), $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}"];

            var model = await _unitOfWork.OrderRepository.FirstOrDefaultAsync(order => order.Id == 1, true, includes);

            var document = new InvoiceDocument { Model = model };

            document.GeneratePdfAndShow();

            return Ok();

            //var tempFilePath = Path.GetTempFileName();

            //document.GeneratePdf(tempFilePath);

            //return File(new FileStream(tempFilePath, FileMode.Open), "application/pdf", "invoice.pdf");

        }
    }
}
