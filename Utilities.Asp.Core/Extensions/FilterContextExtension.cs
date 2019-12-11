using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Utilities.Asp.Core.Extensions
{
    public static class FilterContextExtension
    {
        /// <summary>
        /// Get request header using key.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetRequestHeader(this FilterContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return context.HttpContext.Request.Headers[key];
        }

        /// <summary>
        /// Get request controller.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string? GetRequestController(this FilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return context.RouteData.Values["Controller"]?.ToString();
        }

        /// <summary>
        /// Get request action.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string? GetRequestAction(this FilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return context.RouteData.Values["Action"]?.ToString();
        }

        /// <summary>
        /// Get dependency injection object which is injected from startup as a generic object"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T? GetDependencyInjectionInstance<T>(this FilterContext context)
            where T : class
        {
            var service = GetDependencyInjectionInstance(context, typeof(T));
            return service as T;
        }

        /// <summary>
        /// Get dependency injection object which is injected from startup.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDependencyInjectionInstance(this FilterContext context, Type type)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var requestServices = context.HttpContext.RequestServices;
            var service = requestServices.GetService(type);
            return service;
        }
    }
}