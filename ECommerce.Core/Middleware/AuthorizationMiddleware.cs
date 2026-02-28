using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);

            // فقط إذا لم يتم كتابة الـ response بعد
            if (!httpContext.Response.HasStarted)
            {
                if (httpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync("{\"message\": \"Unauthorized - You must log in to access this resource.\"}");
                }
                else if (httpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync("{\"message\": \"Forbidden - You do not have permission to perform this action.\"}");
                }
            }
        }
    }
}