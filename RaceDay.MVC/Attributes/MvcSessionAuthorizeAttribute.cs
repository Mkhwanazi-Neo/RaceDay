using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.MVC.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MvcSessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public MvcSessionAuthorizeAttribute(string requiredRole = "")
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var userRole = context.HttpContext.Session.GetString("UserRole");

            if (userId == null || string.IsNullOrEmpty(userRole))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (!string.IsNullOrEmpty(_requiredRole) && !userRole.Equals(_requiredRole, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = userRole == "Organiser"
                    ? new RedirectToActionResult("Dashboard", "Organiser", null)
                    : new RedirectToActionResult("Index", "Participant", null);
            }
        }
    }
}