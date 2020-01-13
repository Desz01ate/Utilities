using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Utilities.Asp.Core.Attributes
{
    /// <summary>
    /// Abstract class that provide required signature for manual authorization handler.
    /// </summary>
    public abstract class AuthorizationBasedAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// Called after the action executes, before the action result.
        /// </summary>
        /// <param name="context"></param>
        public abstract void OnActionExecuted(ActionExecutedContext context);
        /// <summary>
        /// Called before the action executes, after model binding is complete.
        /// </summary>
        /// <param name="context"></param>
        public abstract void OnActionExecuting(ActionExecutingContext context);
    }
}