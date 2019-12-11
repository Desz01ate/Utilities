using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Utilities.Asp.Core.Attributes
{
    public abstract class AuthorizationBasedAttribute : Attribute, IActionFilter
    {
        public abstract void OnActionExecuted(ActionExecutedContext context);

        public abstract void OnActionExecuting(ActionExecutingContext context);
    }
}