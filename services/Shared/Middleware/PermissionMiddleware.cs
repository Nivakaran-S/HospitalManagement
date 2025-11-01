using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace Shared.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get user's role
            var userRole = context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userRole))
            {
                // Add role to context items for easy access
                context.Items["UserRole"] = userRole;

                // You can add additional permission checks here
                // based on the endpoint being accessed
            }

            await _next(context);
        }
    }

    // Extension method for easier registration
    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PermissionMiddleware>();
        }
    }

    // Attribute for permission-based authorization
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequirePermissionAttribute : Attribute
    {
        public string Module { get; set; }
        public string Permission { get; set; } // View, Create, Update, Delete

        public RequirePermissionAttribute(string module, string permission)
        {
            Module = module;
            Permission = permission;
        }
    }
}