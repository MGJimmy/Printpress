using Microsoft.AspNetCore.Diagnostics;
using Printpress.Application;
using Printpress.Domain.Enums;

namespace Printpress.API.Middlewares;

public class GlobalExceptionMiddleWare : IExceptionHandler
{
    private readonly ILogger _logger;
    private readonly IHostEnvironment _hostEnvironment;
    public GlobalExceptionMiddleWare(ILogger<GlobalExceptionMiddleWare> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
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
