using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.API.Attributes
{
    public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public SessionAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var userRole = context.HttpContext.Session.GetString("UserRole");

            // 1. Check if user is logged in
            if (userId == null || string.IsNullOrEmpty(userRole))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authentication required. Please log in." });
                return;
            }

            // 2. Check if role matches allowed roles (if specific roles were passed)
            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ObjectResult(new { message = "Forbidden: You do not have permission to access this resource." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
