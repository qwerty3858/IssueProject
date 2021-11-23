using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using IssueProject.Common;

namespace IssueProject.Filters
{
    public class ModelStateValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            var vModelStates = context.ModelState.Values;

            List<string> vErrorData = (
                    from vModelState in context.ModelState.Values
                    from vError in vModelState.Errors
                    select vError.ErrorMessage)
                .ToList();           

            Result vResponse = Result<List<string>>.PrepareFailure("Model State Error", vErrorData);
            context.Result = new OkObjectResult(vResponse)
            {
                ContentTypes = {"application/json"}
            };
        }
    }
}
