using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace TrashMob.Common
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before contrller

            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray());

                var errorResponse = new ErrorResponse();

                foreach (var error in errorsInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new ErrorModel
                        {
                            FieldName = error.Key,
                            Message = subError
                        };

                        errorResponse.Error.Add(errorModel);
                    }

                    context.Result = new BadRequestObjectResult(errorResponse);
                    return;
                }

                await next();

                //after controller  
            }
        }
    }
}
