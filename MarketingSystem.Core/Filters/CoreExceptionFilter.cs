using MarketingSystem.Core.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace MarketingSystem.Core.Filters
{
    public class CoreExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            CoreError dgmError;
            if (context.Exception is CoreException exception)
            {
                var ex = exception;
                context.Exception = null;
                dgmError = ex.Error;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                dgmError = new CoreError("Unauthorized access", StatusCodes.Status401Unauthorized);
            }
            else
            {
                var env = (IHostingEnvironment)context.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment));
                var msg = "An unhandled error occurred.";
                string stack = null;
                if (!env.IsProduction())
                {
                    msg = context.Exception.Message;
                    stack = context.Exception.StackTrace;
                }
                dgmError = new CoreError($"{msg} {stack}");
            }
            context.HttpContext.Response.StatusCode = dgmError.StatusCode;
            context.Result = new JsonResult(dgmError, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            base.OnException(context);
        }
    }
}