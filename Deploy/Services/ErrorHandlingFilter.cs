using Forsvik.Core.Model.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace forsvikapi.Services
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            LogService.StaticError(exception.GetBaseException().Message);

            //context.ExceptionHandled = true; //optional 
        }
    }
}
