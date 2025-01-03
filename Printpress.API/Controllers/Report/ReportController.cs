using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
namespace Printpress.API;

public class ReportController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ReportController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult OrderReport()
    {

        ViewData["Message"] = "Your contact page.";
        return new ViewAsPdf(viewData: ViewData)
        {
            PageSize = Size.A4,
            PageOrientation = Orientation.Portrait,
            CustomSwitches = "--enable-local-file-access",
            UserStyleSheet = "~/lib/bootstrap/dist/css/bootstrap.min.css"

        };


    }


}
