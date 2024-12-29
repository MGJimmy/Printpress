using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Printpress.API;
using Printpress.Application;
using Printpress.Domain.Enums;

public class ResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var hasSkipAttribute = context.ActionDescriptor.EndpointMetadata.OfType<SkipResponseWrapperFilterAttribute>().Any();

        if (context.Result is ObjectResult objectResult && objectResult.Value != null && !hasSkipAttribute)
        {
            var response = new Response<object>(
                ResponseStatus.Success,
                ResponseMessage.Success,
                objectResult.Value
            );

            context.Result = new ObjectResult(response)
            {
                StatusCode = objectResult.StatusCode
            };
        }

        await next();
    }

}
