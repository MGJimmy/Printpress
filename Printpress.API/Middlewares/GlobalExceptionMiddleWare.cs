using Microsoft.AspNetCore.Diagnostics;
using Printpress.Application;
using Printpress.Domain;

namespace Printpress.API;

public class GlobalExceptionMiddleWare : IExceptionHandler
{
    private readonly ILogger _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILocalizationService _localization;
    public GlobalExceptionMiddleWare(
        ILogger<GlobalExceptionMiddleWare> logger, 
        IHostEnvironment hostEnvironment,
        ILocalizationService localization)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
        _localization = localization;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        if (exception is ValidationExeption)
        {
            var response = new Response(ResponseStatus.ValidationFailure, ResponseMessage.ValidationFailure, error: exception.Message);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }


        if (exception is BusinessExceptions)
        {
            string exceptionLocalizedMessage = _localization.Get(exception.Message);

            var response = new Response(ResponseStatus.ValidationFailure, ResponseMessage.ValidationFailure, error: exceptionLocalizedMessage);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }

        if (!_hostEnvironment.IsDevelopment())
        {
            _logger.LogError(exception.ToString());

            var response = new Response(ResponseStatus.ExceptionError, ResponseMessage.InternalServerError);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }

        return false;
    }
}
