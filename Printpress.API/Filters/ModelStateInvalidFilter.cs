using Microsoft.AspNetCore.Mvc.Filters;

namespace Printpress.API
{
    public class ModelStateInvalidFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                   .SelectMany(e => e.Value.Errors.Select(err => err.ErrorMessage).ToList()).ToList();

                context.Result = new JsonResult(new Response(ResponseStatus.ValidationFailure, ResponseMessage.ValidationFailure, errors))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

                return;
            }

            await next();

        }
    }
}
