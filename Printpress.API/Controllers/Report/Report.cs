using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Printpress.API.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class Report(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpGet]
        [Route("generateInvoiceReport/{id}")]
        public async Task<IActionResult> GenerateInvoiceReport(int id)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            string[] includes = [
                     nameof(Order.Client),             
                     $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.Items)}.{nameof(Item.Details)}",
                     $"{nameof(Order.OrderGroups)}.{nameof(OrderGroup.OrderGroupServices)}.{nameof(OrderGroupService.Service)}"
            ];

            var model = await _unitOfWork.OrderRepository.FirstOrDefaultAsync(order => order.Id == id, true, includes);

            var document = new InvoiceDocument { Model = model };

            document.GeneratePdfAndShow();

            return Ok();

            //var tempFilePath = Path.GetTempFileName();

            //document.GeneratePdf(tempFilePath);

            //return File(new FileStream(tempFilePath, FileMode.Open), "application/pdf", "invoice.pdf");

        }
    }
}
